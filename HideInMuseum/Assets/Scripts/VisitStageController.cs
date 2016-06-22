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
    private Image _satisfactionFaceImage;
    [SerializeField]
    private Sprite[] _satisfactionFaceSprites;
    [SerializeField]
    private Entrance _entrance;
    [SerializeField]
    private TextMesh _timeText;
    [SerializeField]
    private Door _door;
    [SerializeField]
    private GroupMovement _leaderPrefab;

    [SerializeField]
    protected GameObject[] _roomsToVisitLogos;
    [SerializeField]
    protected GameObject _goToExitText;
    [SerializeField]
    private GroupUIImage[] _groupsImages;
    [SerializeField]
    private AudioClip _dingDongClip;
    [SerializeField]
    private Vector2 _spawnCooldownRange;

    private List<GroupMovement> _groupsSpawned = new List<GroupMovement>();
    protected InputMode _previousInputMode = InputMode.CameraControls;
    protected GroupMovement _previousGroupMovement = null;
    private AudioSource _source;
    private float _timer;
    private float _spawnCooldown;
    private bool _coroutineStarted = false;
    private bool _doorClosed = false;

    public override void OnMenuBegin()
    {
        foreach (GroupMovement gm in _groupsSpawned)
        {
            Destroy(gm.gameObject);
        }

        _groupsSpawned.Clear();

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

        gameObject.SetActive(false);
        _UIGameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        if (GameManager.Instance.PreviousState != GameState.Paused)
        {
            if(_source == null)
            {
                _source = gameObject.AddComponent<AudioSource>();
            }
            _source.playOnAwake = false;
            _source.loop = false;
            _source.volume = 1.0f;
            _source.spatialBlend = 0.0f;
            _source.spatialize = false;
            _source.clip = _dingDongClip;

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

            for(int i = 0; i < _groupsImages.Length; ++i)
            {
                _groupsImages[i].Initialize(null);
            }

            ExclamationMark.Instance.Reset();
            SpawnNewGroup();
            StartCoroutine(TutorialShow());
            _coroutineStarted = false;
            _timer = 0.0f;
            _spawnCooldown = Random.Range(_spawnCooldownRange.x, _spawnCooldownRange.y);

            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos = _door.transform.position;
            cameraPos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = cameraPos;
            InputHandler.Instance.AdjustCameraTransform();
        }

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

    public void GroupClick(int i)
    {
        if(_groupsSpawned.Count > i)
        {
            InputHandler.Instance.SelectGroup(_groupsSpawned[i]);
        }
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
                Image currentImage = _roomsToVisitLogos[i].GetComponent<Image>();
                currentImage.sprite = GameManager.Instance.GetSpriteByRoomType(group.RoomsToVisit[rooms - j]);
                currentImage.color = group.MyColor;
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
        for(int i = 0; i < _groupsSpawned.Count; ++i)
        {
            _groupsImages[i].Initialize(_groupsSpawned[i]);
        }
        for(int i = _groupsSpawned.Count; i < _groupsImages.Length; ++i)
        {
            _groupsImages[i].Initialize(null);
        }
    }

    void Start()
    {
        _timeText.GetComponent<Renderer>().sortingOrder = 5;
    }

    void Update()
    {
        UpdateSatisfactionUI();
        _timeText.text = Utilities.ConvertSecondsToTimeText(GameManager.Instance.TimeLeft);

        if(InputHandler.Instance.CurrentGroup != null)
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

        if(GameManager.Instance.TimeLeft > 0.0f)
        {
            _timer += Time.deltaTime;
            if(_timer > _spawnCooldown)
            {
                SpawnNewGroup();
            }

            if(_groupsSpawned.Count == 0)
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
                        ExclamationMark.Instance.RemoveGroupWaiting(gm);
                        Destroy(gm.gameObject);
                        _groupsSpawned.RemoveAt(i);
                        i -= 1;
                    }
                }

                for (int i = 0; i < _groupsSpawned.Count; ++i)
                {
                    _groupsImages[i].Initialize(_groupsSpawned[i]);
                }
                for (int i = _groupsSpawned.Count; i < _groupsImages.Length; ++i)
                {
                    _groupsImages[i].Initialize(null);
                }
            }

            if(_groupsSpawned.Count <= 0 && !_coroutineStarted)
            {
                _coroutineStarted = true;
                StartCoroutine(EndStageCoroutine());
            }
        }
    }

    void LateUpdate()
    {
        UpdateGroupsImages();
    }

    IEnumerator EndStageCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.CurrentState = GameState.StatisticsWindow;
    }

    IEnumerator TutorialShow()
    {
        Tutorial.Instance.ShowTutorial(TutorialStage.TS_FirstGroupMovement);
        yield return null;
        while(Tutorial.Instance.IsShown())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        Tutorial.Instance.ShowTutorial(TutorialStage.TS_Exclamation);
        yield return null;
        while (Tutorial.Instance.IsShown())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        Tutorial.Instance.ShowTutorial(TutorialStage.TS_GroupIcons);
        yield return null;
        bool selectGroup = false;
        while (Tutorial.Instance.IsShown())
        {
            selectGroup = true;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        if(selectGroup)
        {
            InputHandler.Instance.SelectGroup(_groupsSpawned[0]);
        }
        Tutorial.Instance.ShowTutorial(TutorialStage.TS_GroupObjectives);
        yield return null;
        while (Tutorial.Instance.IsShown())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        Tutorial.Instance.ShowTutorial(TutorialStage.TS_Satisfaction);
        yield return null;
    }

    void SpawnNewGroup()
    {
        if(_groupsSpawned.Count >= GameManager.Instance.MaxGroupCount || ExclamationMark.Instance.IsGroupWaiting())
        {
            return;
        }

        _timer = 0.0f;
        _spawnCooldown = Random.Range(_spawnCooldownRange.x, _spawnCooldownRange.y);
        GroupMovement gm = (GroupMovement)Instantiate(_leaderPrefab, _entrance.transform.position + Vector3.left * 3.0f, Quaternion.Euler(0, 0, 90));
        _groupsSpawned.Add(gm);
        ExclamationMark.Instance.AddGroupWaiting(gm);
        _source.Play();
        _groupsImages[_groupsSpawned.Count - 1].Initialize(gm);
    }

    void UpdateGroupsImages()
    {
        for(int i = 0; i < _groupsImages.Length; ++i)
        {
            _groupsImages[i].UpdateUI();
        }
    }

    void UpdateSatisfactionUI()
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

        float level = GameManager.Instance.SatisfactionLevel;
        float amplitude = GameManager.Instance.SatisfactionAmplitude;
        if(level < -(0.9f * amplitude))
        {
            _satisfactionFaceImage.sprite = _satisfactionFaceSprites[0];
        }
        else if(level < -(0.5f * amplitude))
        {
            _satisfactionFaceImage.sprite = _satisfactionFaceSprites[1];
        }
        else if (level < (0.5f * amplitude))
        {
            _satisfactionFaceImage.sprite = _satisfactionFaceSprites[2];
        }
        else if (level < (0.9f * amplitude))
        {
            _satisfactionFaceImage.sprite = _satisfactionFaceSprites[3];
        }
        else
        {
            _satisfactionFaceImage.sprite = _satisfactionFaceSprites[4];
        }
    }
}
