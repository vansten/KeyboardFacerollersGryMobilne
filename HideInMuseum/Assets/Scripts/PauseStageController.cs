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

    public override void OnEscapePressed()
    {
        ResumeButtonClick();
    }

    public override void OnUnpaused()
    {
        gameObject.SetActive(false);
    }

    public void ResumeButtonClick()
    {
        GameManager.Instance.Unpause();
    }

    public void MenuButtonClick()
    {
        GameManager.Instance.CurrentState = GameState.Menu;
        gameObject.SetActive(false);
    }
}
