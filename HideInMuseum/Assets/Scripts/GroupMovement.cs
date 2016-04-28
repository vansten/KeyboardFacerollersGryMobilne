using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroupMovement : ObjectBase
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _tolerance;

    private List<Vector3> _path;
    private List<Vector3> _lineRendererPoints;
    private LineRenderer _lineRenderer;
    private Vector3 _prevPosition;
    private float _invertedDistance;
    private float _movingTimer = 0.0f;
    private int _currentPathIndex = 0;
    private int _pointsCount;
    private bool _isMoving = false;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if(_lineRenderer == null)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        _lineRenderer.SetWidth(0.2f, 0.2f);
        _lineRenderer.SetColors(Color.white, Color.white);
        _lineRenderer.sortingOrder = 2;
    }

    void FixedUpdate()
    {
        if(!_isMoving)
        {
            return;
        }

        _movingTimer += Time.deltaTime * _speed * _invertedDistance;
        transform.position = Vector3.Lerp(_prevPosition, _path[_currentPathIndex], _movingTimer);
        UpdateLineRendererPoints();
        if (_movingTimer >= 1.0f)
        {
            _prevPosition = transform.position;
            _movingTimer = 0.0f;
            _currentPathIndex += 1;
            if(_currentPathIndex >= _path.Count)
            {
                _isMoving = false;
                _lineRenderer.SetVertexCount(0);
                _lineRenderer.SetPositions(new Vector3[] { });
                return;
            }
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
        Color c = Color.white;
        c.a = 0.5f;
        _lineRenderer.SetColors(c, c);
        _lineRenderer.SetPositions(new Vector3[] { });
        _lineRenderer.SetVertexCount(0);
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
        _invertedDistance = 1.0f / (_path[0] - transform.position).magnitude;
        _prevPosition = transform.position;
        _lineRenderer.SetColors(Color.white, Color.white);
    }

    public void ClearPath()
    {
        _path.Clear();
        _lineRendererPoints.Clear();
        _isMoving = false;
        _lineRenderer.SetPositions(new Vector3[] { });
        _lineRenderer.SetVertexCount(0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        ClearPath();
    }

    public override void OnDecoratorStagetBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnMenuBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnPaused()
    {
        gameObject.SetActive(false);
    }

    public override void OnStatisticsWindowBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        gameObject.SetActive(true);
    }
}
