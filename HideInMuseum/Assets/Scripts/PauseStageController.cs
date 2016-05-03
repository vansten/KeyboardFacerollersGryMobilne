using UnityEngine;
using System.Collections;

public class PauseStageController : ObjectBase
{
    public override void OnDecoratorStagetBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnMenuBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnPaused()
    {
        gameObject.SetActive(true);
    }

    public override void OnStatisticsWindowBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        gameObject.SetActive(false);
    }

    public void ResumeButtonClick()
    {
        GameManager.Instance.Unpause();
        gameObject.SetActive(false);
    }

    public void RestartButtonClick()
    {
        GameManager.Instance.RestartStage();
    }

    public void MenuButtonClick()
    {
        GameManager.Instance.CurrentState = GameState.Menu;
    }
}
