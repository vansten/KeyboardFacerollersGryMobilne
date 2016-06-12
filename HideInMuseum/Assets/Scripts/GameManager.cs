using UnityEngine;
using System.Collections;
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
public class RoomIcon
{
    public RoomType Type;
    public Sprite Icon;
}

public static class Helpers
{
    private static Dictionary<char, int> _hexToDec = new Dictionary<char, int>();

    public static string ConvertSecondsToTimeText(float secondsLeft)
    {
        string s = "";
        int minutes = (int)(secondsLeft / 60.0f);
        int seconds = (int)(secondsLeft - (minutes * 60));
        s = minutes.ToString("00");
        s += ":";
        s += seconds.ToString("00");
        return s;
    }
    
    public static Color FromHex(string hex)
    {
        if(_hexToDec == null || _hexToDec.Count == 0)
        {
            _hexToDec.Add('0', 0);
            _hexToDec.Add('1', 1);
            _hexToDec.Add('2', 2);
            _hexToDec.Add('3', 3);
            _hexToDec.Add('4', 4);
            _hexToDec.Add('5', 5);
            _hexToDec.Add('6', 6);
            _hexToDec.Add('7', 7);
            _hexToDec.Add('8', 8);
            _hexToDec.Add('9', 9);
            _hexToDec.Add('A', 10);
            _hexToDec.Add('B', 11);
            _hexToDec.Add('C', 12);
            _hexToDec.Add('D', 13);
            _hexToDec.Add('E', 14);
            _hexToDec.Add('F', 15);
            _hexToDec.Add('a', 10);
            _hexToDec.Add('b', 11);
            _hexToDec.Add('c', 12);
            _hexToDec.Add('d', 13);
            _hexToDec.Add('e', 14);
            _hexToDec.Add('f', 15);
        }

        bool alphaIncluded = hex.Length == 8;
        string r = hex.Substring(0, 2);
        string g = hex.Substring(2, 2);
        string b = hex.Substring(4, 2);

        float red = 0.0f;
        float green = 0.0f;
        float blue = 0.0f;

        red = (_hexToDec[r[0]] * 16.0f + _hexToDec[r[1]]) / 255.0f;
        green = (_hexToDec[g[0]] * 16.0f + _hexToDec[g[1]]) / 255.0f;
        blue = (_hexToDec[b[0]] * 16.0f + _hexToDec[b[1]]) / 255.0f;

        float alpha = 1.0f;
        if (alphaIncluded)
        {
            string a = hex.Substring(6, 2);
            alpha = (_hexToDec[a[0]] * 16.0f + _hexToDec[a[1]]) / 255.0f;
        }

        return new Color(red, green, blue, alpha);
    }
}

public class GameManager : Singleton<GameManager>
{
    #region Private variables

    [SerializeField]
    private GameObject _museumObject;
    [SerializeField]
    protected List<GroupMovement> _groupLeaders;
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
    public List<RoomIcon> Rooms;
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
        set
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

    public float TotalMoney
    {
        get;
        set;
    }

    public int GroupsHandled
    {
        get;
        set;
    }

    #endregion

    #region Unity methods

    void Start()
    {
        CurrentState = GameState.Menu;
        _counterText.gameObject.SetActive(false);
        TotalMoney = PlayerPrefs.GetFloat("TotalMoneyEarned", 0.0f);
        Debug.Log(TotalMoney);
    }

    void Update()
    {
        if(_currentState == GameState.DecoratorStage || _currentState == GameState.VisitStage)
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
        PlayerPrefs.SetFloat("TotalMoneyEarned", TotalMoney);
        PlayerPrefs.Save();
    }

    IEnumerator OnApplicationPause(bool pauseStatus)
    {
        yield return null;
        yield return null;
        yield return null;

        if (pauseStatus)
        {
            CurrentState = GameState.Paused;
        }
        else
        {
            Unpause();
        }
    }

    #endregion

    #region GameManager methods

    public Sprite GetSpriteByRoomType(RoomType rt)
    {
        foreach(RoomIcon ri in Rooms)
        {
            if(ri.Type == rt)
            {
                return ri.Icon;
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
        RoomType random = (RoomType) UnityEngine.Random.Range(0, 5);
        for (int i = 0; i < group.GroupCount + 1; ++i)
        {
            while (group.RoomsToVisit.Contains(random))
            {
                random = (RoomType)UnityEngine.Random.Range(0, 6);
            }
            group.RoomsToVisit.Add(random);            
        }
    }

    public void DecreaseSatisfaction(SatisfactionStage stage, Vector3 position)
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
    }

    public void IncreaseSatisfaction(SatisfactionStage stage, Vector3 position)
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
