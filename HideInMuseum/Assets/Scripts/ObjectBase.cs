using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour
{
    protected virtual void Start()
    {
        GameManager.Instance.OnDecoratorStageBegin += OnDecoratorStagetBegin;
        GameManager.Instance.OnMenuBegin += OnMenuBegin;
        GameManager.Instance.OnPaused += OnPaused;
        GameManager.Instance.OnStatisticsWindowsBegin += OnStatisticsWindowBegin;
        GameManager.Instance.OnVisitStageBegin += OnVisitStageBegin;
    }

    public virtual void OnVisitStageBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnVisitStageBegin called");
#endif
    }

    public virtual void OnDecoratorStagetBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnDecoratorStageBegin called");
#endif
    }

    public virtual void OnMenuBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnMenuBegin called");
#endif
    }

    public virtual void OnStatisticsWindowBegin()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnStatisticsWindowBegin called");
#endif
    }

    public virtual void OnPaused()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning("Base function for OnPaused called");
#endif
    }
}
