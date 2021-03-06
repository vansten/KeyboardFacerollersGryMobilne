﻿using UnityEngine;
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
    private Camera _mainCamera;
    [SerializeField]
    private LayerMask _clickableLayers;
    [SerializeField]
    private LayerMask _layersContinuePath;
    [SerializeField]
    private Vector2 _zoomRange;
    [SerializeField]
    private float _yAxisCameraAbsolute;
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
    public GroupMovement CurrentGroup
    {
        get { return _currentGroup; }
    }
    private Ray _rayToCast;
    private Vector3 _currentPosition;
    private Vector3 _positionChange;
    private Vector3 _prevPosition;
    private float _aspectRatio;
    private float _halfBgSize;
    private InputMode _currentMode;
    public InputMode CurrentMode
    {
        get { return _currentMode; }
    }
    private bool _inputEnded;
    private bool _continuePath;

    #endregion

    #region Unity methods

    void Start()
    {
        _aspectRatio = (float)Screen.width / (float)Screen.height;
        _halfBgSize = 0.5f * _bgSize;
        _mainCamera.orthographicSize = (_zoomRange.y + _zoomRange.x) * 0.5f;
    }

    void OnDisable()
    {
        if (_currentGroup != null)
        {
            _currentGroup.ClearPath();
        }
    }

    void Update()
    {
        if(GameManager.Instance.CurrentState != GameState.DecoratorStage && GameManager.Instance.CurrentState != GameState.VisitStage)
        {
            return;
        }

        if(Tutorial.Instance.IsPopupActive())
        {
            if(_currentGroup != null && _continuePath)
            {
                _currentGroup.EndPath();
            }
            _currentMode = InputMode.CameraControls;
            return;
        }

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
            _rayToCast = _mainCamera.ScreenPointToRay(Input.mousePosition);
            CastRay();
            _prevPosition = Input.mousePosition;
        }

        float wheelValue = Input.mouseScrollDelta.y;
        _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize - wheelValue * _zoomStep, _zoomRange.x, _zoomRange.y);

        if(Input.GetMouseButton(0))
        {
            _positionChange = Input.mousePosition - _prevPosition;
            _prevPosition = Input.mousePosition;
            _rayToCast = _mainCamera.ScreenPointToRay(Input.mousePosition);
            CastRayContinue();
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
            return;
        }

        if (touches.Length == 1)
        {
            //Single finger
            Touch t0 = touches[0];
            if (t0.phase == TouchPhase.Began)
            {
                _rayToCast = _mainCamera.ScreenPointToRay(t0.position);
                CastRay();
            }
            else if(t0.phase == TouchPhase.Canceled || t0.phase == TouchPhase.Ended)
            {
                _inputEnded = true;
            }
            else if(t0.phase == TouchPhase.Moved)
            {
                _rayToCast = _mainCamera.ScreenPointToRay(t0.position);
                CastRayContinue();
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
            Vector3 prevT1Position = t1.position - t1.deltaPosition;
            Vector3 currentT2Position = t2.position;
            Vector3 prevT2Position = t2.position - t2.deltaPosition;
            float currentDistanceBetweenTouches = Vector3.Distance(currentT1Position, currentT2Position);
            float prevDistanceBetweenTouches = Vector3.Distance(prevT1Position, prevT2Position);
            float distanceChange = prevDistanceBetweenTouches - currentDistanceBetweenTouches;
            _mainCamera.orthographicSize = Mathf.Clamp(_mainCamera.orthographicSize + Mathf.Sign(distanceChange) * _zoomStep, _zoomRange.x, _zoomRange.y);
            _positionChange = Vector3.zero;
        }
    }

    #endregion

    #region Input handler methods

    void CastRay()
    {
        //Do raycast and check if we want to change mode
        RaycastHit2D[] hits = Physics2D.RaycastAll(_rayToCast.origin, _rayToCast.direction, float.MaxValue, _clickableLayers);

        _currentMode = InputMode.CameraControls;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.activeInHierarchy)
            {

                Icon i = hit.collider.GetComponent<Icon>();
                if(i != null)
                {
                    _currentGroup = null;
                    _currentMode = InputMode.CameraControls;
                    i.Tap();
                    break;
                }
                else
                {
                    _currentGroup = hit.collider.GetComponent<GroupMovement>();
                    if(_currentGroup == null)
                    {
                        _currentGroup = hit.collider.transform.parent.GetComponent<GroupMovement>();
                    }
                    
                    if(_currentGroup != null)
                    {
                        _currentGroup.StartPath();
                        _currentMode = InputMode.GroupControls;
                    }
                }
            }
        }

        if(_currentMode == InputMode.CameraControls)
        {
            _currentGroup = null;
        }
    }

    void CastRayContinue()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayToCast.origin, _rayToCast.direction, float.MaxValue, _layersContinuePath);
        if(hit.collider != null)
        {
            _continuePath = true;
        }
    }

    void UpdateCameraControls()
    {
        float multiplier = 8.0f + (8.0f - _mainCamera.orthographicSize);
        if (multiplier < 1.0f)
        {
            multiplier = 1.0f;
        }

        _positionChange /= multiplier;

        if (_invertedControls)
        {
            _mainCamera.transform.position -= _positionChange;
        }
        else
        {
            _mainCamera.transform.position += _positionChange;
        }
    }

    void UpdateGroupControls()
    {
        if(_currentGroup == null)
        {
            _currentMode = InputMode.CameraControls;
            return;
        }

        if(_inputEnded)
        {
            _currentGroup.EndPath();
        }
        
        if(_continuePath)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(_rayToCast.origin, _rayToCast.direction, float.MaxValue, _layersContinuePath);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.activeInHierarchy)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        _currentGroup.WrongPath();
                        break;
                    }
                }
            }

            _currentGroup.ContinuePath(_mainCamera.ScreenToWorldPoint(_currentPosition));
        }
    }

    public void AdjustCameraTransform()
    {
        Vector3 cameraPosition = _mainCamera.transform.position;
        //Adjust camera no matter how is the ortho size (showing only background (no clear color) indepentant to zooming, using constraints points isn't independant)
        float size = _mainCamera.orthographicSize;
        float minX, minY, maxX, maxY;
        minX = _aspectRatio * size - _halfBgSize - _cameraError;
        maxX = _halfBgSize - _aspectRatio * size + _cameraError;
        minY = size - _halfBgSize - _cameraError;
        maxY = _halfBgSize - size + _cameraError;
        _mainCamera.transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, minX, maxX),
                                                        Mathf.Clamp(cameraPosition.y, minY, maxY),
                                                        cameraPosition.z);            
    }

    public void SelectGroup(GroupMovement gm)
    {
        _currentGroup = gm;
        if (_currentGroup != null)
        {
            Vector3 cameraPos = _mainCamera.transform.position;
            cameraPos.x = _currentGroup.transform.position.x;
            cameraPos.y = _currentGroup.transform.position.y;
            _mainCamera.transform.position = cameraPos;
            AdjustCameraTransform();
        }
    }

    #endregion
}
