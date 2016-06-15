using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GyroscopeStageController : Singleton<GyroscopeStageController>
{
    [SerializeField]
    private SpriteRenderer _exhibitSpriteRenderer;
    [SerializeField]
    private Transform _rotationPoint;
    [SerializeField]
    private GameObject _crackedPrefab;
    [SerializeField]
    private Text _timerText;

    private List<GameObject> _crackedExhibits = new List<GameObject>();
    private List<GameObject> _crackedSpawned = new List<GameObject>();
    private GameObject _exhibitObject;
    private GroupMovement _group;
    private float _timer = 0.0f;
    private float _cameraSize;
    private bool _countdown = true;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(Sprite exhibit, GameObject go, GroupMovement group)
    {
        _exhibitSpriteRenderer.sprite = exhibit;
        _rotationPoint.gameObject.SetActive(false);
        _exhibitObject = go;
        Time.timeScale = 0.0f;
        gameObject.SetActive(true);
        InputHandler.Instance.enabled = false;
        _rotationPoint.rotation = Quaternion.identity;
        _rotationPoint.Rotate(0.0f, 0.0f, Random.Range(-1.0f, 1.0f));
        _group = group;
        _timer = 3.0f;
        _countdown = true;
        _cameraSize = Camera.main.orthographicSize;
        Camera.main.orthographicSize = 12.0f;
        StartCoroutine(CountdownCoroutine());
    }

    public void Reset()
    {
        foreach(GameObject go in _crackedExhibits)
        {
            go.SetActive(true);
        }

        foreach(GameObject go in _crackedSpawned)
        {
            Destroy(go);
        }

        _crackedExhibits.Clear();
        _crackedSpawned.Clear();
    }

    void Update()
    {

        transform.position = Camera.main.transform.position;
        transform.position -= new Vector3(0.0f, 0.0f, transform.position.z);

        if (_countdown)
        {
            return;
        }

        Vector3 accelerometerInput = Input.acceleration;
        Vector3 rotation = Vector3.zero;
        accelerometerInput.x += 0.1f;
        if(accelerometerInput.x < 0.0f)
        {
            rotation.z = 1.0f;
        }
        else if(accelerometerInput.x > 0.0f)
        {
            rotation.z = -1.0f;
        }
        _rotationPoint.Rotate(rotation * 60.0f * Time.unscaledDeltaTime);

        float sign = 0.0f;
        float eulerZ = _rotationPoint.rotation.eulerAngles.z;
        if(eulerZ > 0.0f && eulerZ < 180.0f)
        {
            sign = 1.0f;
        }
        else if(eulerZ > 180.0f && eulerZ < 360.0f)
        {
            sign = -1.0f;
        }
        else if(eulerZ < 0.0f && eulerZ > -180.0f)
        {
            sign = -1.0f;
        }
        else if(eulerZ < -180.0f && eulerZ > -360.0f)
        {
            sign = 1.0f;
        }
        else
        {
            sign = 0.0f;
        }

        _rotationPoint.Rotate(0.0f, 0.0f, Time.unscaledDeltaTime * 30.0f * sign);

        _timerText.text = (5.0f - _timer).ToString("0");

        eulerZ = _rotationPoint.rotation.eulerAngles.z;
        if(eulerZ > 75.0f && eulerZ < 285.0f)
        {
            _exhibitObject.SetActive(false);
            GameObject go = (GameObject)Instantiate(_crackedPrefab, _exhibitObject.transform.position, _exhibitObject.transform.rotation);
            _timer = 0.0f;
            _crackedExhibits.Add(_exhibitObject);
            _crackedSpawned.Add(go);
            Time.timeScale = 1.0f;
            InputHandler.Instance.enabled = true;
            GameManager.Instance.ExhibitCracked(_group);
            Camera.main.orthographicSize = _cameraSize;
            gameObject.SetActive(false);
        }

        _timer += Time.unscaledDeltaTime;
        if(_timer > 5.0f)
        {
            _timer = 0.0f;
            Time.timeScale = 1.0f;
            InputHandler.Instance.enabled = true;
            Camera.main.orthographicSize = _cameraSize;
            gameObject.SetActive(false);
        }
    }

    IEnumerator CountdownCoroutine()
    {
        int i = 0;
        while(i < 3)
        {
            float timer = 0.0f;
            while(timer <= 1.0f)
            {
                timer += Time.unscaledDeltaTime;
                _timerText.fontSize = (int)(180.0f * timer);
                yield return null;
            }

            i += 1;
            _timerText.text = (3 - i).ToString();
            yield return null;
        }

        _countdown = false;
        _timer = 0.0f;
        _rotationPoint.gameObject.SetActive(true);
    }
}
