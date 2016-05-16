using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
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

public static class Helpers
{
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

    [Range(0.1f, 10.0f)]
    public float SatisfactionAmplitude = 5.0f;

    public Action OnVisitStageBegin;
    public Action OnDecoratorStageBegin;
    public Action OnMenuBegin;
    public Action OnStatisticsWindowsBegin;
    public Action OnPaused;

    #endregion

    #region Properties

    public GameState CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _previousState = _currentState;
            _currentState = value;
            switch(_currentState)
            {
                case GameState.DecoratorStage:
                    Time.timeScale = 1.0f;
                    StageTime = _decoratorStageTime;
                    _museumObject.SetActive(true);
                    if(OnDecoratorStageBegin != null)
                    {
                        OnDecoratorStageBegin();
                    }
                    break;
                case GameState.VisitStage:
                    Time.timeScale = 1.0f;
                    StageTime = _visitStageTime;
                    _museumObject.SetActive(true);
                    SatisfactionLevel = 0.0f;
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
                    CurrentState = GameState.StatisticsWindow;
                }
            }
        }
    }

    #endregion

    #region Unity methods

    void Start()
    {
        CurrentState = GameState.Menu;
        _counterText.gameObject.SetActive(false);
    }

    void Update()
    {
        if(_currentState == GameState.DecoratorStage || _currentState == GameState.VisitStage)
        {
            TimeLeft -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            switch (_currentState)
            {
                case GameState.Menu:
                    Application.Quit();
                    break;
                case GameState.DecoratorStage:
                case GameState.StatisticsWindow:
                case GameState.VisitStage:
                    CurrentState = GameState.Menu;
                    break;
                case GameState.Paused:
                    Unpause();
                    break;
            }
        }
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

    public void UpdateGroupMissionsDispaly(GroupMovement group)
    {
        _visitStageController.FillRoomsToVisit(group);
    }

    public void GenerateMission(GroupMovement group)
    {
        group.RoomsToVisit.Clear();
        RoomType random = (RoomType) UnityEngine.Random.Range(0, 6);
        for (int i = 0; i < group.GroupCount + 1; ++i)
        {
            while (group.RoomsToVisit.Contains(random))
            {
                random = (RoomType)UnityEngine.Random.Range(0, 7);
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

    public void RestartStage()
    {
        GameState stateToRestart = _previousState;
        CurrentState = GameState.Menu;
        CurrentState = stateToRestart;
    }

    #endregion
}
