using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CuratorMovement : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _tolerance;

    private List<Vector3> _path = new List<Vector3>();
    private List<Vector3> _lineRendererPoints = new List<Vector3>();
    private Color _colorToSet = Color.white;
    private Vector3 _lastPosition;
    private Vector3 _lastLineRendererPosition;
    private float _moveTimer;
    private float _distanceToNextNode = 0.0f;
    private int _currentPathIndex = 0;
    private int _positionsCount = 0;
    private bool _isMoving;

    void Start()
    {
        _lineRenderer.sortingOrder = 2;
    }

    void Update()
    {
        if(_isMoving)
        {
            if(_currentPathIndex < _path.Count)
            {
                _moveTimer += Time.deltaTime * _speed / _distanceToNextNode;
                transform.position = Vector3.Lerp(_lastPosition, _path[_currentPathIndex], _moveTimer);
                UpdateLineRendererPoints();
                if (_moveTimer >= 1.0f)
                {
                    _moveTimer = 0.0f;
                    _currentPathIndex += 1;
                    _lastPosition = transform.position;
                    if (_currentPathIndex >= _path.Count)
                    {
                        _isMoving = false;
                    }
                    else
                    {
                        _distanceToNextNode = Vector3.Distance(_path[_currentPathIndex], _lastPosition);
                    }
                }
            }
        }
    }

    void UpdateLineRendererPoints()
    {
        int index = 0;
        float closestDist = float.MaxValue;
        for(int i = 0; i < _lineRendererPoints.Count; ++i)
        {
            float dist = Vector3.Distance(transform.position, _lineRendererPoints[i]);
            if (dist < closestDist)
            {
                index = i;
                closestDist = dist;
            }
        }

        if(closestDist > 0.2f)
        {
            return;
        }

        _lineRendererPoints = _lineRendererPoints.GetRange(index, _lineRendererPoints.Count - index);

        _lineRenderer.SetVertexCount(_lineRendererPoints.Count);
        _lineRenderer.SetPositions(_lineRendererPoints.ToArray());
    }

    public void StartPath(Vector3 position)
    {
        _isMoving = false;
        _path.Clear();
        _positionsCount = 0;
        Color c = Color.white;
        c.a = 0.5f;
        _lineRenderer.SetVertexCount(0);
        _lineRenderer.SetColors(c, c);
        _lineRendererPoints.Clear();
        _lastLineRendererPosition = position;
        _colorToSet = Color.white;
    }

    public void ContinuePath(Vector3 position)
    {
        if(_path.Count > 0)
        {
            if (Vector3.Distance(position,_path[_path.Count - 1]) < _tolerance)
            {
                return;
            }
        }
        float dist = Vector3.Distance(_lastLineRendererPosition, position);
        if(dist > 0.2f)
        {
            Vector3 diff = (position - _lastLineRendererPosition);
            int k = (int)(dist * 10.0f);
            diff *= 1.0f / k;
            for(int i = 1; i < k; ++i)
            {
                _lineRenderer.SetVertexCount(_positionsCount + 1);
                _lineRenderer.SetPosition(_positionsCount, _lastLineRendererPosition + diff * i);
                _lineRendererPoints.Add(_lastLineRendererPosition + diff * i);
                _positionsCount += 1;
            }
        }
        _lineRenderer.SetVertexCount(_positionsCount + 1);
        _lineRenderer.SetPosition(_positionsCount, position);
        _lineRendererPoints.Add(position);
        _lastLineRendererPosition = position;
        _positionsCount += 1;
        _path.Add(position);
    }

    public void EndPath(Vector3 position)
    {
        _lineRenderer.SetColors(_colorToSet, _colorToSet);
        _path.Add(position);
        _isMoving = true;
        _currentPathIndex = 0;
        _moveTimer = 0.0f;
        _lastPosition = transform.position;
        _distanceToNextNode = Vector3.Distance(_path[0], transform.position);
    }

    public void WrongPath()
    {
        _colorToSet = Color.red;
        Color c = Color.red;
        c.a = 0.5f;
        _lineRenderer.SetColors(c, c);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        _isMoving = false;
        _path.Clear();
        _lineRenderer.SetVertexCount(0);
    }
}