using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupMovement : MonoBehaviour
{
    public GameObject GroupMemberTemplate;
    public int GroupCount;
    [SerializeField]
    private List<GroupMember> _groupMembers = new List<GroupMember>();

    public List<Sprite> Sprites
    {
        get { return _sprites; }
    }
    [SerializeField]
    private List<Sprite> _sprites;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _tolerance;

    private List<Vector3> _path;
    private List<Vector3> _lineRendererPoints;
    private LineRenderer _lineRenderer;
    private Color _myLineColor;
    private Color _colorToSet = Color.red;
    private Vector3 _prevPosition;
    private Vector3 _targetUp;
    private Vector3 _initUp;
    private float _invertedDistance;
    private float _movingTimer = 0.0f;
    private float _standingTimer = 0.0f;
    private int _currentPathIndex = 0;
    private int _pointsCount;

    public bool IsMoving
    {
        get{ return _isMoving;}
    }
    private bool _isMoving = false;

    public Vector3 MovementDelta
    {
        get { return _movementDelta; }
    }
    private Vector3 _movementDelta;

    void OnEnable()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr != null && _sprites != null && _sprites.Count > 0)
        {
            sr.sprite = _sprites[Random.Range(0, _sprites.Count)];
        }

        _lineRenderer = GetComponent<LineRenderer>();
        if(_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        _lineRenderer.SetWidth(0.5f, 0.5f);
        _myLineColor = new Color(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), 1.0f);
        _lineRenderer.SetColors(_myLineColor, _myLineColor);
        _lineRenderer.sortingOrder = 2;

        transform.up = Vector2.up;
        transform.position = Vector3.zero;
        ClearPath();
        
        GenerateGroup();
    }

    void GenerateGroup()
    {
        if (GroupCount <= 0 || _groupMembers.Count == GroupCount) return;

        for (int i = 0; i < GroupCount; ++i)
        {
            Transform previousInQueue;
            previousInQueue = i == 0 ? transform : _groupMembers[i - 1].transform;
            GameObject go = Instantiate(GroupMemberTemplate, previousInQueue.GetChild(0).position, previousInQueue.rotation) as GameObject;
            go.transform.parent = previousInQueue.parent;
            GroupMember member = go.GetComponent<GroupMember>();
            if (member != null)
            {
                _groupMembers.Add(member);
                member.GroupLeader = this;
                member.PreviousMemberHook = previousInQueue.GetChild(0);
            }
            SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.sprite = _sprites[Random.Range(0, _sprites.Count)];
            }
        }
    } 
    

    void FixedUpdate()
    {
        if(!_isMoving)
        {
            _standingTimer += Time.deltaTime;
            if(_standingTimer > 5.0f)
            {
                if(_standingTimer < 10.0f)
                {
                    GameManager.Instance.DecreaseSatisfaction(SatisfactionStage.SS_First, transform.position);
                }
                else if(_standingTimer < 20.0f)
                {
                    GameManager.Instance.DecreaseSatisfaction(SatisfactionStage.SS_Second, transform.position);
                }
                else
                {
                    GameManager.Instance.DecreaseSatisfaction(SatisfactionStage.SS_Third, transform.position);
                }
            }

            return;
        }

        _movingTimer += Time.deltaTime * _speed * _invertedDistance;
        Vector3 newPosition = Vector3.Lerp(_prevPosition, _path[_currentPathIndex], _movingTimer);
        _movementDelta = newPosition - transform.position;
        transform.position = newPosition;
        transform.up = Vector3.Lerp(_initUp, _targetUp, _movingTimer * 4.0f);
        UpdateLineRendererPoints();
        if (_movingTimer >= 1.0f)
        {
            _prevPosition = transform.position;
            _movingTimer = 0.0f;
            _currentPathIndex += 1;
            _initUp = transform.up;
            if (_currentPathIndex >= _path.Count)
            {
                _standingTimer = 0.0f;
                _isMoving = false;
                _lineRenderer.SetVertexCount(0);
                _lineRenderer.SetPositions(new Vector3[] { });
                return;
            }
            _targetUp = -(_path[_currentPathIndex] - _prevPosition).normalized;
            _invertedDistance = 1.0f / (_path[_currentPathIndex] - _prevPosition).magnitude;
        }
    }

    void UpdateLineRendererPoints()
    {
        float tolerance = 0.2f;
        int index = 0;
        for(int i = 0; i < _lineRendererPoints.Count; ++i)
        {
            if(Vector3.Distance(transform.position, _lineRendererPoints[i]) > tolerance)
            {
                index = i - 1;
                break;
            }
        }

        if(index >= 0)
        {
            _lineRendererPoints = _lineRendererPoints.GetRange(index, _lineRendererPoints.Count - index);

            _lineRenderer.SetVertexCount(_lineRendererPoints.Count);
            _lineRenderer.SetPositions(_lineRendererPoints.ToArray());
        }
    }

    public void StartPath()
    {
        _path = new List<Vector3>();
        _isMoving = false;
        _pointsCount = 0;
        _lineRendererPoints = new List<Vector3>();
        _colorToSet = _myLineColor;
        Color c = _myLineColor;
        c.a = 0.5f;
        _lineRenderer.SetColors(c, c);
        _lineRenderer.SetPositions(new Vector3[] { });
        _lineRenderer.SetVertexCount(0);
        _initUp = transform.up;
    }

    public void ContinuePath(Vector3 position)
    {
        position.z = 0.0f;
        if(_path.Count > 0)
        {
            float dist = Vector3.Distance(position, _path[_path.Count - 1]);
            if (dist < _tolerance)
            {
                return;
            }
        }

        if (_lineRendererPoints.Count > 0)
        {
            Vector3 lastLineRendererPosition = _lineRendererPoints[_lineRendererPoints.Count - 1];
            float distance = Vector3.Distance(_lineRendererPoints[_lineRendererPoints.Count - 1], position);
            if (distance > 0.2f)
            {
                Vector3 diff = (position - lastLineRendererPosition);
                int k = (int)(distance * 10.0f);
                diff *= 1.0f / k;
                for (int i = 1; i < k; ++i)
                {
                    _lineRenderer.SetVertexCount(_pointsCount + 1);
                    _lineRenderer.SetPosition(_pointsCount, lastLineRendererPosition + diff * i);
                    _lineRendererPoints.Add(lastLineRendererPosition + diff * i);
                    _pointsCount += 1;
                }
            }
        }

        _pointsCount += 1;
        _lineRenderer.SetVertexCount(_pointsCount);
        _lineRenderer.SetPosition(_pointsCount - 1, position);
        _lineRendererPoints.Add(position);
        _path.Add(position);
    }

    public void EndPath()
    {
        if(_path.Count == 0)
        {
            return;
        }
        _currentPathIndex = 0;
        _isMoving = true;
        _standingTimer = 0.0f;
        _movingTimer = 0.0f;
        _invertedDistance = 1.0f / (_path[0] - transform.position).magnitude;
        _prevPosition = transform.position;
        _targetUp = -(_path[0] - transform.position).normalized;
        _lineRenderer.SetColors(_colorToSet, _colorToSet);
    }

    public void WrongPath()
    {
        _colorToSet = Color.red;
        Color c = _colorToSet;
        c.a = 0.5f;
        _lineRenderer.SetColors(c, c);
    }
    
    public void ClearPath()
    {
        if(_path == null)
        {
            return;
        }
        _path.Clear();
        _lineRendererPoints.Clear();
        _isMoving = false;
        _lineRenderer.SetPositions(new Vector3[] { });
        _lineRenderer.SetVertexCount(0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.GetComponent<GroupMovement>() != null)
        {
            GameManager.Instance.SatisfactionLevel -= 0.75f;
        }
        ClearPath();
    }
}
