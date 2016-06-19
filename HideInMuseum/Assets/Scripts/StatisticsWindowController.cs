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
        bool win = GameManager.Instance.SatisfactionLevel > -GameManager.Instance.SatisfactionAmplitude && GameManager.Instance.GroupsHandled > 0;
        _failedSucceededText.text = win ? "KONIEC DNIA" : "ODWIEDZAJĄCY BYLI ZAWIEDZENI";
        _failedSucceededText.color = win ? Utilities.ColorFromHex("a1ff64") : Utilities.ColorFromHex("ff6666"); //green : red

        _groupsHandledText.text = GameManager.Instance.GroupsHandled.ToString();
        _satisfactionLevelText.text = SatisfactionToString(GameManager.Instance.SatisfactionLevel);
        float multiplier = 1.0f / (GameManager.Instance.SatisfactionAmplitude * 2.0f);
        float maxCashPerGroup = 10.0f * GameManager.Instance.GetNumberOfUnlockedRooms();
        int moneyEarned = (int)Mathf.Lerp(0.0f, maxCashPerGroup * GameManager.Instance.GroupsHandled, (GameManager.Instance.SatisfactionLevel + GameManager.Instance.SatisfactionAmplitude) * multiplier);
        GameManager.Instance.TotalMoney += moneyEarned;
        _moneyEarnedText.text = "$" + moneyEarned.ToString();

        gameObject.SetActive(true);
        GyroscopeStageController.Instance.Reset();
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
        if(level < -(0.9f * GameManager.Instance.SatisfactionAmplitude))
        {
            return "PORAŻKA";
        }
        else if(level < -(0.5f * GameManager.Instance.SatisfactionAmplitude))
        {
            return "ZAWÓD";
        }
        else if(level < (0.5f * GameManager.Instance.SatisfactionAmplitude))
        {
            return "NIC SIĘ NIE STAŁO";
        }
        else if(level < (0.9f * GameManager.Instance.SatisfactionAmplitude))
        {
            return "NIEŹLE";
        }
        else
        {
            return "SUPER";
        }
    }
}