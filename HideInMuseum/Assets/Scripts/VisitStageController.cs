using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VisitStageController : ObjectBase
{
    [SerializeField]
    private GameObject _UIGameObject;
    [SerializeField]
    private Image _satisfactionLevelImage;
    [SerializeField]
    private Text _timerText;

    public override void OnMenuBegin()
    {
        gameObject.SetActive(false);
        _UIGameObject.SetActive(false);
    }

    public override void OnPaused()
    {
        base.OnPaused();
        _UIGameObject.SetActive(false);
    }

    public override void OnStatisticsWindowBegin()
    {
        MonoBehaviour[] mbs = GetComponentsInChildren<MonoBehaviour>();
        foreach(MonoBehaviour mb in mbs)
        {
            mb.enabled = false;
        }
        _UIGameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        gameObject.SetActive(true);

        MonoBehaviour[] mbs = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour mb in mbs)
        {
            mb.enabled = true;
        }
        _UIGameObject.SetActive(true);
    }

    public void PauseButtonClick()
    {
        GameManager.Instance.CurrentState = GameState.Paused;
    }

    void Update()
    {
        float tmp = GameManager.Instance.SatisfactionLevel + 5.0f;
        _satisfactionLevelImage.fillAmount = Mathf.Clamp01(tmp * 0.1f);
        _satisfactionLevelImage.color = new Color(0.1f * (10.0f - tmp), 0.1f * tmp, 0.0f, 1.0f);
        _timerText.text = Helpers.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);
    }
}
