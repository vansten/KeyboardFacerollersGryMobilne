using UnityEngine;
using System.Collections;

public enum InputMode
{
    CameraControls,
    GroupControls
}

public class InputHandler : Singleton<InputHandler>
{
    #region Private Variables

    [SerializeField]
    private Vector2 _zoomRange;
    [SerializeField]
    private float _zoomStep;
    [SerializeField]
    private float _bgSize;
    [SerializeField]
    [Tooltip("How much can camera show clear color? (outside background, for example setting 0.5 - it can be 0.5 in x and 0.5 in y different that corner of background)")]
    private float _cameraError;
    [SerializeField]
    private bool _invertedControls;

    private GroupMovement _currentGroup;
    private Ray _rayToCast;
    private Vector3 _currentPosition;
    private Vector3 _positionChange;
    private Vector3 _prevPosition;
    private float _aspectRatio;
    private float _halfBgSize;
    private InputMode _currentMode;
    private bool _inputEnded;
    private bool _continuePath;

    #endregion

    #region Unity methods

    void Start()
    {
        _aspectRatio = (float)Screen.width / (float)Screen.height;
        _halfBgSize = 0.5f * _bgSize;
    }

    void Update()
    {
        _inputEnded = false;
        _continuePath = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        ProcessMouse();
#elif UNITY_ANDROID || UNITY_IOS
        ProcessTouches();
#endif

        _positionChange.z = 0.0f;

        switch(_currentMode)
        {
            case InputMode.CameraControls:
                UpdateCameraControls();
                break;
            case InputMode.GroupControls:
                UpdateGroupControls();
                break;
        }
    }

    void LateUpdate()
    {
        AdjustCameraTransform();
    }

    #endregion

    #region Process input methods

    void ProcessMouse()
    {
        _currentPosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            _rayToCast = Camera.main.ScreenPointToRay(Input.mousePosition);
            CastRay();
            _prevPosition = Input.mousePosition;
        }

        float wheelValue = Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - wheelValue * _zoomStep, _zoomRange.x, _zoomRange.y);

        if(Input.GetMouseButton(0))
        {
            _positionChange = Input.mousePosition - _prevPosition;
            _positionChange *= 0.1f * Camera.main.orthographicSize;
            _prevPosition = Input.mousePosition;
            _continuePath = true;
        }

        if(Input.GetMouseButtonUp(0))
        {
            _inputEnded = true;
        }
    }

    void ProcessTouches()
    {
        Touch[] touches = Input.touches;
        if (touches.Length == 0)
        {
            //There is no touch to check
            _inputEnded = true;
            return;
        }

        if (touches.Length == 1)
        {
            //Single finger
            Touch t0 = touches[0];
            if (t0.phase == TouchPhase.Began)
            {
                _rayToCast = Camera.main.ScreenPointToRay(t0.position);
                CastRay();
            }
            else if(t0.phase == TouchPhase.Canceled || t0.phase == TouchPhase.Ended)
            {
                _inputEnded = true;
            }
            else if(t0.phase == TouchPhase.Moved)
            {
                _continuePath = true;
            }

            _currentPosition = t0.position;
            _positionChange = t0.deltaPosition;
        }
        else if(touches.Length >= 2)
        {
            _inputEnded = true;

            //Two or more fingers
            //Get first two fingers and check theirs position change in order to determine distance between theirs position in this and previous frame
            //Then change size of camera to zoom in or out
            Touch t1 = touches[0];
            Touch t2 = touches[1];
            Vector3 currentT1Position = t1.position;
            Vector3 prevT1Position = t1.position + t1.deltaPosition;
            Vector3 currentT2Position = t2.position;
            Vector3 prevT2Position = t2.position + t2.deltaPosition;
            float currentDistanceBetweenTouches = Vector3.Distance(currentT1Position, currentT2Position);
            float prevDistanceBetweenTouches = Vector3.Distance(prevT1Position, prevT2Position);
            float distanceChange = currentDistanceBetweenTouches - prevDistanceBetweenTouches;
            Camera.main.orthographicSize += distanceChange;
        }
    }

    #endregion

    #region Input handler methods

    void CastRay()
    {
        //Do raycast and check if we want to change mode
        RaycastHit2D hit = Physics2D.Raycast(_rayToCast.origin, _rayToCast.direction);
        if(hit.collider != null)
        {
            //Check if it's character
            _currentGroup = hit.collider.GetComponent<GroupMovement>();
            if(_currentGroup != null)
            {
                _currentGroup.StartPath();
                _currentMode = InputMode.GroupControls;
            }
            else
            {
                _currentMode = InputMode.CameraControls;
            }
        }
        else
        {
            _currentMode = InputMode.CameraControls;
        }
    }

    void UpdateCameraControls()
    {
        _positionChange /= Camera.main.orthographicSize;
        if(_invertedControls)
        {
            Camera.main.transform.position -= _positionChange;
        }
        else
        {
            Camera.main.transform.position += _positionChange;
        }
    }

    void UpdateGroupControls()
    {
        if(_currentGroup == null)
        {
            return;
        }

        if(_inputEnded)
        {
            _currentGroup.EndPath();
        }
        
        if(_continuePath)
        {
            _currentGroup.ContinuePath(Camera.main.ScreenToWorldPoint(_currentPosition));
        }
    }

    void AdjustCameraTransform()
    {
        //Adjust camera no matter how is the ortho size (showing only background (no clear color) indepentant to zooming, using constraints points isn't independant)
        Vector3 cameraPosition = Camera.main.transform.position;
        float size = Camera.main.orthographicSize;
        float minX, minY, maxX, maxY;
        minX = _aspectRatio * size - _halfBgSize - _cameraError;
        maxX = _halfBgSize - _aspectRatio * size + _cameraError;
        minY = size - _halfBgSize - _cameraError;
        maxY = _halfBgSize - size + _cameraError;
        Camera.main.transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, minX, maxX),
                                                        Mathf.Clamp(cameraPosition.y, minY, maxY),
                                                        cameraPosition.z);
    }

    #endregion
}
