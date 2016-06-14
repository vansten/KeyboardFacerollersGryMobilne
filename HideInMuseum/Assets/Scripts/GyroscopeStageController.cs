using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GyroscopeStageController : Singleton<GyroscopeStageController>
{
    [SerializeField]
    private SpriteRenderer _exhibitSpriteRenderer;
    [SerializeField]
    private Transform _rotationPoint;
    [SerializeField]
    private GameObject _crackedPrefab;

    private List<GameObject> _crackedExhibits = new List<GameObject>();
    private List<GameObject> _crackedSpawned = new List<GameObject>();
    private GameObject _exhibitObject;
    private GroupMovement _group;
    float _timer = 0.0f;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(Sprite exhibit, GameObject go, GroupMovement group)
    {
        _exhibitSpriteRenderer.sprite = exhibit;
        _exhibitObject = go;
        Time.timeScale = 0.0f;
        gameObject.SetActive(true);
        InputHandler.Instance.enabled = false;
        _rotationPoint.rotation = Quaternion.identity;
        _rotationPoint.Rotate(0.0f, 0.0f, Random.Range(-1.0f, 1.0f));
        _group = group;
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
/*#if UNITY_EDITOR
        float z = 0.0f;
        if(Input.GetKey(KeyCode.A))
        {
            z += 1.0f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            z -= 1.0f;
        }
        Vector3 tmpGyro = new Vector3(0.0f, 0.0f, z);
        _rotationPoint.Rotate(tmpGyro);
#endif*/

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
            gameObject.SetActive(false);
        }

        _timer += Time.unscaledDeltaTime;
        if(_timer > 5.0f)
        {
            _timer = 0.0f;
            Time.timeScale = 1.0f;
            InputHandler.Instance.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
