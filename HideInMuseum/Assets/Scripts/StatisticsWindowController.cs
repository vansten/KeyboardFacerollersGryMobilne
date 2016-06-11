using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatisticsWindowController : ObjectBase
{
    [SerializeField]
    private Text _failedSucceededText;
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
        bool win = GameManager.Instance.SatisfactionLevel > -5.0f && GameManager.Instance.GroupsHandled > 0;
        _failedSucceededText.text = win ? "SUCCEEDED" : "FAILED";
        _failedSucceededText.color = win ? Helpers.FromHex("a1ff64") : Helpers.FromHex("ff6666"); //green : red

        _groupsHandledText.text = GameManager.Instance.GroupsHandled.ToString();
        _satisfactionLevelText.text = SatisfactionToString(GameManager.Instance.SatisfactionLevel);
        float multiplier = 1.0f / (GameManager.Instance.SatisfactionAmplitude * 2.0f);
        float moneyEarned = Mathf.Lerp(0.0f, 50.0f * GameManager.Instance.GroupsHandled, (GameManager.Instance.SatisfactionLevel + GameManager.Instance.SatisfactionAmplitude) * multiplier);
        GameManager.Instance.TotalMoney += moneyEarned;
        _moneyEarnedText.text = "$" + moneyEarned.ToString("0.00");

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

    private string SatisfactionToString(float level)
    {
        if(level < -4.5f)
        {
            return "Very unhappy";
        }
        else if(level < -2.5f)
        {
            return "Unhappy";
        }
        else if(level < 2.5f)
        {
            return "Neutral";
        }
        else if(level < 4.5f)
        {
            return "Happy";
        }
        else
        {
            return "Very happy";
        }
    }
}