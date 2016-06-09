using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameManager.Instance.OnDecoratorStageBegin += OnDecoratorStagetBegin;
        GameManager.Instance.OnMenuBegin += OnMenuBegin;
        GameManager.Instance.OnPaused += OnPaused;
        GameManager.Instance.OnStatisticsWindowsBegin += OnStatisticsWindowBegin;
        GameManager.Instance.OnVisitStageBegin += OnVisitStageBegin;
        GameManager.Instance.OnUnpaused += OnUnpaused;
    }

    public virtual void OnEnable()
    {
        GameManager.Instance.OnEscapePressed += OnEscapePressed;
    }

    public virtual void OnDisable()
    {
        if(GameManager.InstanceExists())
        {
            GameManager.Instance.OnEscapePressed -= OnEscapePressed;
        }
    }

    public virtual void OnVisitStageBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnVisitStageBegin called in " + gameObject.name);
#endif
    }

    public virtual void OnDecoratorStagetBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnDecoratorStageBegin called in " + gameObject.name);
#endif
    }

    public virtual void OnMenuBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnMenuBegin called in " + gameObject.name);
#endif
    }

    public virtual void OnStatisticsWindowBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnStatisticsWindowBegin called in " + gameObject.name);
#endif
    }

    public virtual void OnPaused()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnPaused called in " + gameObject.name);
#endif
    }

    public virtual void OnEscapePressed()
    {
        GameManager.Instance.CurrentState = GameState.Paused;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnEscapePressed called in " + gameObject.name);
#endif
    }

    public virtual void OnUnpaused()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for Unpaused called in " + gameObject.name);
#endif
    }
}
