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
    private Image _satisfactionSadImage;
    [SerializeField]
    private Image _satisfactionHappyImage;
    [SerializeField]
    private Entrance _entrance;
    [SerializeField]
    private Door _door;
    [SerializeField]
    private GroupMovement _leaderPrefab;

    [SerializeField]
    protected GameObject[] _roomsToVisitLogos;
    [SerializeField]
    protected GameObject _goToExitText;
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private float _spawnCooldown;

    private List<GroupMovement> _groupsSpawned = new List<GroupMovement>();
    protected InputMode _previousInputMode = InputMode.CameraControls;
    protected GroupMovement _previousGroupMovement = null;
    private float _timer;
    private bool _coroutineStarted = false;
    private bool _doorClosed = false;

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
        foreach (GroupMovement gm in _groupsSpawned)
        {
            Destroy(gm.gameObject);
        }

        _groupsSpawned.Clear();
        _UIGameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        if (GameManager.Instance.PreviousState != GameState.Paused)
        {
            gameObject.SetActive(true);

            MonoBehaviour[] mbs = GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour mb in mbs)
            {
                mb.enabled = true;
            }

            _entrance.VisitStage = this;
            foreach (GroupMovement gm in _groupsSpawned)
            {
                Destroy(gm.gameObject);
            }

            _doorClosed = false;
            _door.Open();
            _groupsSpawned = new List<GroupMovement>();
            SpawnNewGroup();
            _coroutineStarted = false;
        }

        _timer = 0.0f;
        _UIGameObject.SetActive(true);
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
                //_roomsToVisitLogos[i].GetComponentInChildren<Text>().text = GetRoomShortName(group.RoomsToVisit[rooms - j]);                
                _roomsToVisitLogos[i].GetComponent<Image>().sprite = GameManager.Instance.GetSpriteByRoomType(group.RoomsToVisit[rooms - j]);
            }
        }
        _goToExitText.SetActive(rooms == 0);
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

    public void GroupLeft(GroupMovement group)
    {
        GameManager.Instance.GroupsHandled += 1;
        _groupsSpawned.Remove(group);
        Destroy(group.gameObject);
    }

    void Update()
    {
        float tmp = GameManager.Instance.SatisfactionLevel + 5.0f;
        tmp *= 0.1f;
        _satisfactionLevelImage.fillAmount = Mathf.Clamp01(tmp);

        if (tmp <= 0.5f)
        {
            _satisfactionLevelImage.color = tmp * 2.0f * Color.yellow + (1.0f - tmp * 2.0f) * Color.red;
        }
        else
        {
            _satisfactionLevelImage.color = (tmp - 0.5f) * 2.0f * Color.green + (1.0f - (tmp - 0.5f) * 2.0f) * Color.yellow;
        }

        _satisfactionSadImage.color = new Color(1, 1, 1, 1.0f - tmp);
        _satisfactionHappyImage.color = new Color(1, 1, 1, tmp);

        _timerText.text = Helpers.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);

        if(Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.SatisfactionLevel += 5.0f;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.SatisfactionLevel = -4.5f;
        }

        if(InputHandler.Instance.CurrentMode == InputMode.GroupControls)
        {
            if(_previousGroupMovement != InputHandler.Instance.CurrentGroup && InputHandler.Instance.CurrentGroup != null)
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

        if(GameManager.Instance.TimeLeft > 0.0f)
        {
            _timer += Time.deltaTime;
            if(_timer > _spawnCooldown)
            {
                SpawnNewGroup();
            }
        }
        else
        {
            if(!_doorClosed)
            {
                _doorClosed = true;
                _door.Close();

                //Check if any group left outside the museum
                for(int i = 0; i < _groupsSpawned.Count; ++i)
                {
                    if(_groupsSpawned[i].CurrentRoomType == RoomType.Entrance)
                    {
                        GroupMovement gm = _groupsSpawned[i];
                        Destroy(gm.gameObject);
                        _groupsSpawned.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
            if(_groupsSpawned.Count <= 0 && !_coroutineStarted)
            {
                _coroutineStarted = true;
                StartCoroutine(EndStageCoroutine());
            }
        }
    }

    IEnumerator EndStageCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.CurrentState = GameState.StatisticsWindow;
    }

    void SpawnNewGroup()
    {
        if(_groupsSpawned.Count >= 3)
        {
            return;
        }

        _timer = 0.0f;
        GroupMovement gm = (GroupMovement)Instantiate(_leaderPrefab, _entrance.transform.position, Quaternion.Euler(0, 0, 90));
        _groupsSpawned.Add(gm);
    }
}
