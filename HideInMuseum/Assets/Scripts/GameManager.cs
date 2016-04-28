using UnityEngine;
using System.Collections;
using System;

public enum GameState
{
    VisitStage,
    DecoratorStage,
    Paused,
    Menu,
    StatisticsWindow
}

public class GameManager : Singleton<GameManager>
{
    #region Private variables

    private GameState _previousState;
    private GameState _currentState;
    private float _satisfactionLevel;
    private float _timeLeft;
    private float _stageTime;

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
                    if(OnDecoratorStageBegin != null)
                    {
                        OnDecoratorStageBegin();
                    }
                    break;
                case GameState.VisitStage:
                    if(OnVisitStageBegin != null)
                    {
                        OnVisitStageBegin();
                    }
                    break;
                case GameState.Menu:
                    if(OnMenuBegin != null)
                    {
                        OnMenuBegin();
                    }
                    break;
                case GameState.Paused:
                    if(OnPaused != null)
                    {
                        OnPaused();
                    }
                    break;
                case GameState.StatisticsWindow:
                    if(OnStatisticsWindowsBegin != null)
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
            _timeLeft = _stageTime;
        }
    }

    public float TimeLeft
    {
        get
        {
            return _timeLeft;
        }
    }

    #endregion

    #region Unity methods

    void Start()
    {
        CurrentState = GameState.VisitStage;
        SatisfactionLevel = 0.0f;
    }

    void Update()
    {
        if(_currentState == GameState.DecoratorStage || _currentState == GameState.VisitStage)
        {
            _timeLeft -= Time.deltaTime;
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
                    CurrentState = _previousState;
                    break;
            }
        }
    }

    #endregion
}
