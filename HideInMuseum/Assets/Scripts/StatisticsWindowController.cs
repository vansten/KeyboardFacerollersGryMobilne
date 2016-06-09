using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatisticsWindowController : ObjectBase
{
    [SerializeField]
    private Text _failedSucceededText;
    [SerializeField]
    private Text _timeLeftText;
    [SerializeField]
    private Text _groupsHandledText;
    [SerializeField]
    private Text _satisfactionLevelText;
    [SerializeField]
    private Text _moneyEarnedText;

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
        gameObject.SetActive(false);
    }

    public override void OnEscapePressed()
    {
        GameManager.Instance.CurrentState = GameState.Menu;
    }

    public override void OnStatisticsWindowBegin()
    {
        bool win = GameManager.Instance.SatisfactionLevel > -5.0f;
        _failedSucceededText.text = win ? "SUCCEEDED" : "FAILED";
        _failedSucceededText.color = win ? Color.green : Color.red;

        _timeLeftText.text = "Time left: " + Helpers.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);
        _groupsHandledText.text = "Groups handled: " + 0;
        _satisfactionLevelText.text = "Satisfaction level: " + GameManager.Instance.SatisfactionLevel.ToString("0.00");
        float multiplier = 1.0f / (GameManager.Instance.SatisfactionAmplitude * 2.0f);
        float moneyEarned = Mathf.Lerp(0.0f, 50.0f * GameManager.Instance.GroupsHandled, (GameManager.Instance.SatisfactionLevel + GameManager.Instance.SatisfactionAmplitude) * multiplier);
        GameManager.Instance.TotalMoney += moneyEarned;
        _moneyEarnedText.text = "Money earned: $" + moneyEarned.ToString("0.00");

        gameObject.SetActive(true);
    }

    public override void OnVisitStageBegin()
    {
        gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        GameManager.Instance.CurrentState = GameState.Menu;
    }
}