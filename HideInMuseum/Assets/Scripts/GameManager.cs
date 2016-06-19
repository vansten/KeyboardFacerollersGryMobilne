using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using AStar;
using UnityEngine.UI;

public enum GameState
{
    VisitStage,
    DecoratorStage,
    Paused,
    Menu,
    StatisticsWindow
}

public enum SatisfactionStage
{
    SS_First,
    SS_Second,
    SS_Third
}

[Serializable]
public class RoomInfo
{
    public RoomType Type;
    public Room Room;
    public Sprite Icon;
    public Sprite ShopImage;
    public string Name;
    public int MoneyToUnlock;
}

public class GameManager : Singleton<GameManager>
{
    #region Private variables

    [SerializeField]
    private GameObject _museumObject;
    [SerializeField]
    private Text _counterText;
    private GameState _previousState;
    private GameState _currentState;
    [SerializeField]
    private float _visitStageTime = 300.0f;
    [SerializeField]
    private float _decoratorStageTime = 600.0f;
    private float _satisfactionLevel;
    private float _timeLeft;
    private float _stageTime;
    [SerializeField]
    protected VisitStageController _visitStageController;

    #endregion

    #region Public variables

    public Grid MainGrid;
    public List<RoomInfo> Rooms;
    public int MaxGroupCount;

    [Range(0.1f, 10.0f)]
    public float SatisfactionAmplitude = 5.0f;

    public Action OnVisitStageBegin;
    public Action OnDecoratorStageBegin;
    public Action OnMenuBegin;
    public Action OnStatisticsWindowsBegin;
    public Action OnPaused;
    public Action OnUnpaused;
    public Action OnEscapePressed;

    #endregion

    #region Properties

    public GameState PreviousState
    {
        get
        {
            return _previousState;
        }
    }

    public GameState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if(_currentState == value)
            {
                return;
            }

            _previousState = _currentState;
            _currentState = value;
            switch(_currentState)
            {
                case GameState.DecoratorStage:
                    Time.timeScale = 1.0f;
                    if(_previousState != GameState.Paused)
                    {
                        StageTime = _decoratorStageTime;
                    }
                    _museumObject.SetActive(true);
                    if(OnDecoratorStageBegin != null)
                    {
                        OnDecoratorStageBegin();
                    }
                    break;
                case GameState.VisitStage:
                    Time.timeScale = 1.0f;
                    if(_previousState != GameState.Paused)
                    {
                        StageTime = _visitStageTime;
                        GroupsHandled = 0;
                        SatisfactionLevel = 0.0f;
                    }
                    _museumObject.SetActive(true);
                    if (OnVisitStageBegin != null)
                    {
                        OnVisitStageBegin();
                    }
                    break;
                case GameState.Menu:
                    Time.timeScale = 1.0f;
                    _museumObject.SetActive(false);
                    if (OnMenuBegin != null)
                    {
                        OnMenuBegin();
                    }
                    break;
                case GameState.Paused:
                    if(OnPaused != null)
                    {
                        Time.timeScale = 0.0f;
                        OnPaused();
                    }
                    break;
                case GameState.StatisticsWindow:
                    Time.timeScale = 1.0f;
                    if (OnStatisticsWindowsBegin != null)
                    {
                        OnStatisticsWindowsBegin();
                    }
                    break;
            }
        }
    }

    public float SatisfactionLevel
    {
        get
        {
            return _satisfactionLevel;
        }
        private set
        {
            _satisfactionLevel = value;
            if(_satisfactionLevel <= -SatisfactionAmplitude)
            {
                CurrentState = GameState.StatisticsWindow;
            }
            _satisfactionLevel = Mathf.Clamp(_satisfactionLevel, -SatisfactionAmplitude, SatisfactionAmplitude);
        }
    }

    public float StageTime
    {
        set
        {
            _stageTime = Mathf.Clamp(value, 0.0f, float.MaxValue);
            TimeLeft = _stageTime;
        }
    }

    public float TimeLeft
    {
        get
        {
            return _timeLeft;
        }
        private set
        {
            _timeLeft = Mathf.Clamp(value, 0.0f, float.MaxValue);
            if(value <= 0.0f)
            {
                if(_currentState == GameState.DecoratorStage)
                {
                    CurrentState = GameState.VisitStage;
                }
                else if(_currentState == GameState.VisitStage)
                {
                    //CurrentState = GameState.StatisticsWindow;
                }
            }
        }
    }

    public int TotalMoney
    {
        get;
        set;
    }

    public int GroupsHandled
    {
        get;
        set;
    }

    private bool _soundOn;
    public bool SoundOn
    {
        get
        {
            return _soundOn;
        }
        set
        {
            _soundOn = value;
            AudioListener.volume = _soundOn ? 1.0f : 0.0f;
        }
    }

    #endregion

    #region Unity methods

    protected override void Awake()
    {
        base.Awake();
        SoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        if(PlayerPrefs.GetInt("WasFirstLaunch", 0) == 1)
        {
            for (int i = 0; i < Rooms.Count; ++i)
            {
                Rooms[i].Room.Unlocked = PlayerPrefs.GetInt("Room" + i, 0) == 1;
            }
        }
        else
        {
            PlayerPrefs.SetInt("WasFirstLaunch", 1);
            foreach (RoomInfo ri in Rooms)
            {
                if (ri.Type != RoomType.Animations && ri.Type != RoomType.Tapes)
                {
                    ri.Room.Unlocked = true;
                }
                PlayerPrefs.SetInt("Room" + Rooms.IndexOf(ri), ri.Room.Unlocked ? 1 : 0);
            }
        }
    }

    void Start()
    {
        CurrentState = GameState.Menu;
        _counterText.gameObject.SetActive(false);
        TotalMoney = PlayerPrefs.GetInt("TotalMoneyEarned", 0);
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void Update()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        if (_currentState == GameState.DecoratorStage || _currentState == GameState.VisitStage)
        {
            TimeLeft -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("TotalMoneyEarned", TotalMoney);
        PlayerPrefs.SetInt("SoundOn", SoundOn ? 1 : 0);
        for(int i = 0; i < Rooms.Count; ++i)
        {
            PlayerPrefs.SetInt("Room" + i, Rooms[i].Room.Unlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    IEnumerator OnApplicationPause(bool pauseStatus)
    {
        yield return null;
        yield return null;
        yield return null;

        if (pauseStatus && CurrentState == GameState.VisitStage)
        {
            CurrentState = GameState.Paused;
        }
    }

    #endregion

    #region GameManager methods

    public int GetNumberOfUnlockedRooms()
    {
        return Rooms.Where(r => r.Room.Unlocked).ToList().Count;
    }

    public Sprite GetSpriteByRoomType(RoomType rt)
    {
        foreach(RoomInfo ri in Rooms)
        {
            if(ri.Type == rt)
            {
                return ri.Icon;
            }
        }

        return null;
    }

    public Room GetRoomByRoomType(RoomType rt)
    {
        foreach(RoomInfo ri in Rooms)
        {
            if(ri.Type == rt)
            {
                return ri.Room;
            }
        }

        return null;
    }

    public void UpdateGroupMissionsDispaly(GroupMovement group)
    {
        if(InputHandler.Instance.CurrentGroup == group)
        {
            _visitStageController.FillRoomsToVisit(group);
        }
    }

    public void GenerateMission(GroupMovement group)
    {
        group.RoomsToVisit.Clear();
        Room room = Rooms[UnityEngine.Random.Range(0, Rooms.Count)].Room;
        for(int i = 0; i < group.GroupCount + 1; ++i)
        {
            while(group.RoomsToVisit.Contains(room.Type) || !room.Unlocked)
            {
                room = Rooms[UnityEngine.Random.Range(0, Rooms.Count)].Room;
            }
            group.RoomsToVisit.Add(room.Type);
        }
    }

    public void DecreaseSatisfaction(SatisfactionStage stage, GroupMovement group)
    {
        float value = Time.deltaTime;
        switch (stage)
        {
            case SatisfactionStage.SS_First:
                value *= 0.025f;
                break;
            case SatisfactionStage.SS_Second:
                value *= 0.075f;
                break;
            case SatisfactionStage.SS_Third:
                value *= 0.125f;
                break;
        }
        SatisfactionLevel -= value;
        group.TogglePlusParticles(false);
        group.ToggleMinusParticles(true);
    }

    public void IncreaseSatisfaction(SatisfactionStage stage, GroupMovement group)
    {
        float value = Time.deltaTime;
        switch (stage)
        {
            case SatisfactionStage.SS_First:
                value *= 0.5f;
                break;
            case SatisfactionStage.SS_Second:
                value *= 0.25f;
                break;
            case SatisfactionStage.SS_Third:
                value *= 0.125f;
                break;
        }
        SatisfactionLevel += value;
        group.ToggleMinusParticles(false);
        group.TogglePlusParticles(true);
    }

    public void ExhibitCracked(GroupMovement group)
    {
        float value = (SatisfactionLevel + SatisfactionAmplitude) * 0.5f;
        SatisfactionLevel -= value;
        group.TogglePlusParticles(false);
        group.ToggleMinusParticles(true);
        StartCoroutine(StopMinusParticlesCoroutine(group));
    }

    IEnumerator StopMinusParticlesCoroutine(GroupMovement group)
    {
        yield return new WaitForSeconds(1.0f);
        group.ToggleMinusParticles(false);
    }

    public void Unpause()
    {
        if(CurrentState != GameState.Paused)
        {
            return;
        }

        StartCoroutine(UnpauseCoroutine());
    }

    IEnumerator UnpauseCoroutine()
    {
        OnUnpaused();

        if (_previousState == GameState.DecoratorStage || _previousState == GameState.VisitStage)
        {
            Vector3 initScale = Vector3.one * 0.75f;
            Vector3 targetScale = Vector3.one * 1.25f;
            float timer = 0.0f;
            int counter = 0;

            _counterText.gameObject.SetActive(true);

            while (counter < 3)
            {
                timer = 0.0f;
                _counterText.rectTransform.localScale = initScale;
                _counterText.text = (3 - counter).ToString();

                while (timer <= 1.0f)
                {
                    _counterText.rectTransform.localScale = Vector3.Lerp(initScale, targetScale, timer);
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }

                _counterText.rectTransform.localScale = targetScale;
                counter += 1;

                yield return null;
            }

            _counterText.gameObject.SetActive(false);
        }

        CurrentState = _previousState;
    }

    #endregion
}
