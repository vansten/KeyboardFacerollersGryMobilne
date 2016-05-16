using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;

public class VisitStageController : ObjectBase
{
    [SerializeField]
    private GameObject _UIGameObject;
    [SerializeField]
    private Image _satisfactionLevelImage;

    [SerializeField]
    protected GameObject[] _roomsToVisitLogos;
    [SerializeField]
    private Text _timerText;

    protected InputMode _previousInputMode = InputMode.CameraControls;
    protected GroupMovement _previousGroupMovement = null;


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

    public void FillRoomsToVisit(GroupMovement group)
    {
        int rooms = group.RoomsToVisit.Count;
        for (int i = 2, j = 1; i >= 0; --i ,++j)
        {
            if (j > rooms)
            {
                _roomsToVisitLogos[i].SetActive(false);
            }
            else
            {
                _roomsToVisitLogos[i].SetActive(true);
                _roomsToVisitLogos[i].GetComponentInChildren<Text>().text = group.RoomsToVisit[rooms - j].ToString().Substring(0,1);                
            }
        }
    }

    void Update()
    {
        float tmp = GameManager.Instance.SatisfactionLevel + 5.0f;
        _satisfactionLevelImage.fillAmount = Mathf.Clamp01(tmp * 0.1f);
        _satisfactionLevelImage.color = new Color(0.1f * (10.0f - tmp), 0.1f * tmp, 0.0f, 1.0f);
        _timerText.text = Helpers.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);

        if (InputHandler.Instance.CurrentMode != _previousInputMode)
        {
            if (InputHandler.Instance.CurrentMode == InputMode.GroupControls && _previousGroupMovement != InputHandler.Instance.CurrentGroup)
            {
                _roomsToVisitLogos[0].transform.parent.gameObject.SetActive(true);
                FillRoomsToVisit(InputHandler.Instance.CurrentGroup);
            }
            else
            {
                _roomsToVisitLogos[0].transform.parent.gameObject.SetActive(false);
            }
        }
        _previousInputMode = InputHandler.Instance.CurrentMode;
        _previousGroupMovement = InputHandler.Instance.CurrentGroup;
    }
}
