using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _minRestriction;
    [SerializeField]
    private Transform _maxRestriction;
    [SerializeField]
    private bool _inverted;

    private CuratorMovement _currentCharacterAffected;
    private Vector3 _previousMousePosition;
    private Vector3 _deltaMousePosition;
    private bool _isDown = false;
    private bool _mouseDown = false;
    private bool _mouseBeingHeldDown = false;
    private bool _mouseUp = false;

    void Update()
    {
        _mouseDown = Input.GetMouseButtonDown(0);
        _mouseBeingHeldDown = Input.GetMouseButton(0);
        _mouseUp = Input.GetMouseButtonUp(0);
        ProcessCuratorsMovement();
        ProcessCameraScrolling();
    }

    void ProcessCuratorsMovement()
    {
        if(_mouseDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit.collider != null)
            {
                if (_currentCharacterAffected == null)
                {
                    _currentCharacterAffected = hit.collider.gameObject.GetComponent<CuratorMovement>();
                    if (_currentCharacterAffected != null)
                    {
                        Vector3 position;
                        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        position.z = 0.0f;
                        _currentCharacterAffected.StartPath(position);
                    }
                }
            }
        }
        else if(_mouseBeingHeldDown)
        {
            if(_currentCharacterAffected != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    //Check if sth was hit or just continue
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        _currentCharacterAffected.WrongPath();
                    }
                }

                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0.0f;
                _currentCharacterAffected.ContinuePath(position);
            }
        }
        else if(_mouseUp)
        {
            _deltaMousePosition = Vector3.zero;

            if (_currentCharacterAffected != null)
            {
                Vector3 position;
                position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0.0f;
                _currentCharacterAffected.EndPath(position);
            }
            _currentCharacterAffected = null;
        }
    }

    void ProcessCameraScrolling()
    {
        if(_currentCharacterAffected != null)
        {
            _previousMousePosition = Input.mousePosition;
            return;
        }

        if (!_mouseBeingHeldDown && !_mouseDown)
        {
            _previousMousePosition = Input.mousePosition;
        }
        _deltaMousePosition = Input.mousePosition - _previousMousePosition;
        if(_inverted)
        {
            _deltaMousePosition = -_deltaMousePosition;
        }
        _previousMousePosition = Input.mousePosition;
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position + _deltaMousePosition * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, _minRestriction.position.x, _maxRestriction.position.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _minRestriction.position.y, _maxRestriction.position.y);
        transform.position = newPosition;
    }
}