using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VisitStageController : ObjectBase
{
    [SerializeField]
    private GameObject _UIGameObject;
    [SerializeField]
    private Image _satisfactionLevelImage;
    [SerializeField]
    private Entrance _entrance;
    [SerializeField]
    private GroupMovement _leaderPrefab;

    [SerializeField]
    protected GameObject[] _roomsToVisitLogos;
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private float _spawnCooldown;

    protected InputMode _previousInputMode = InputMode.CameraControls;
    protected GroupMovement _previousGroupMovement = null;
    private float _timer;
    private int _groupCount;

    public override void OnMenuBegin()
    {
        gameObject.SetActive(false);
        _UIGameObject.SetActive(false);
    }

    public override void OnPaused()
    {
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
        _timer = 0.0f;

        gameObject.SetActive(true);

        MonoBehaviour[] mbs = GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour mb in mbs)
        {
            mb.enabled = true;
        }

        _UIGameObject.SetActive(true);
        
        if (GameManager.Instance.PreviousState != GameState.Paused)
        {
            _entrance.VisitStage = this;
            _groupCount = 0;
            SpawnNewGroup();
        }
    }

    public override void OnUnpaused()
    {
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
                _roomsToVisitLogos[i].GetComponentInChildren<Text>().text = GetRoomShortName(group.RoomsToVisit[rooms - j]);                
            }
        }
    }

    string GetRoomShortName(RoomType rt)
    {
        string name = rt.ToString();
        string toRet = string.Empty;
        int i = 0;
        while(i < name.Length)
        {
            if(char.IsUpper(name[i]))
            {
                toRet += name[i];
            }
            i += 1;
        }

        return toRet;
    }

    public void GroupLeft()
    {
        GameManager.Instance.GroupsHandled += 1;
        _groupCount -= 1;
    }

    void Update()
    {
        float tmp = GameManager.Instance.SatisfactionLevel + 5.0f;
        _satisfactionLevelImage.fillAmount = Mathf.Clamp01(tmp * 0.1f);
        _satisfactionLevelImage.color = new Color(0.1f * (10.0f - tmp), 0.1f * tmp, 0.0f, 1.0f);
        _timerText.text = Helpers.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);

        /*if (InputHandler.Instance.CurrentMode != _previousInputMode)
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
        }*/

        if(InputHandler.Instance.CurrentMode == InputMode.GroupControls)
        {
            if(_previousGroupMovement != InputHandler.Instance.CurrentGroup)
            {
                if(_previousGroupMovement == null)
                {
                    _roomsToVisitLogos[0].transform.parent.gameObject.SetActive(true);
                }
                FillRoomsToVisit(InputHandler.Instance.CurrentGroup);
            }
        }
        else
        {
            _roomsToVisitLogos[0].transform.parent.gameObject.SetActive(false);
        }

        _previousInputMode = InputHandler.Instance.CurrentMode;
        _previousGroupMovement = InputHandler.Instance.CurrentGroup;

        _timer += Time.deltaTime;
        if(_timer > _spawnCooldown)
        {
            SpawnNewGroup();
        }
    }

    void SpawnNewGroup()
    {
        if(_groupCount >= 3)
        {
            return;
        }

        _groupCount += 1;
        _timer = 0.0f;
        GroupMovement gm = (GroupMovement)Instantiate(_leaderPrefab, _entrance.transform.position, Quaternion.Euler(0, 0, 90));
        gm.transform.parent = transform;
    }
}
