using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _minRestriction;
    [SerializeField]
    private Transform _maxRestriction;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Vector3 _offset;

    private CharacterMovement _currentCharacterAffected;
    private Vector3 _newPosition;
    private Vector3 _cameraPositionBeforeLerp;
    private float _lerpTimer = 0.0f;

    void Start()
    {
        transform.position = _target.position + _offset;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WP8_1 || UNITY_WSA_8_1 || UNITY_WSA_10_0
        Touch t = Input.GetTouch(0);
        if(t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
#endif
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                if (_currentCharacterAffected == null)
                {
                    _currentCharacterAffected = hit.collider.gameObject.GetComponent<CharacterMovement>();
                    if (_currentCharacterAffected != null)
                    {
                        Vector3 position;
#if UNITY_EDITOR
                        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WP8_1 || UNITY_WSA_8_1 || UNITY_WSA_10_0
                        position = Camera.main.ScreenToWorldPoint(t.position);
#endif
                        position.z = 0.0f;
                        _currentCharacterAffected.StartPath(position);
                    }
                }
                else
                {
                    //Check if sth was hit or just continue
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        _currentCharacterAffected.WrongPath();
                    }

                    Vector3 position;
#if UNITY_EDITOR
                    position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WP8_1 || UNITY_WSA_8_1 || UNITY_WSA_10_0
                    position = Camera.main.ScreenToWorldPoint(t.position);
#endif
                    position.z = 0.0f;
                    _currentCharacterAffected.ContinuePath(position);
                }
            }
            else if(_currentCharacterAffected != null)
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0.0f;
                _currentCharacterAffected.ContinuePath(position);
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(_currentCharacterAffected != null)
            {
                Vector3 position;
#if UNITY_EDITOR
                position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID || UNITY_IOS || UNITY_WP8_1 || UNITY_WSA_8_1 || UNITY_WSA_10_0
                 position = Camera.main.ScreenToWorldPoint(t.position);
#endif
                position.z = 0.0f;
                _currentCharacterAffected.EndPath(position);
            }
            _currentCharacterAffected = null;
        }
    }

    void LateUpdate()
    {
        Vector2 targetPositionXY = new Vector2(_target.position.x, _target.position.y);
        Vector2 cameraPositionXY = new Vector2(transform.position.x, transform.position.y);
        if (Mathf.Abs(targetPositionXY.x - cameraPositionXY.x) < 1.0f && (Mathf.Abs(targetPositionXY.y - cameraPositionXY.y) < 1.0f))
        {
            _cameraPositionBeforeLerp = transform.position;
            _lerpTimer = 0.0f;
            return;
        }

        _lerpTimer += Time.deltaTime;

        _newPosition = _target.position + _offset;
        _newPosition.x = Mathf.Clamp(_newPosition.x, _minRestriction.position.x, _maxRestriction.position.x);
        _newPosition.y = Mathf.Clamp(_newPosition.y, _minRestriction.position.y, _maxRestriction.position.y);
        transform.position = Vector3.Lerp(_cameraPositionBeforeLerp, _newPosition, _lerpTimer);
    }
}