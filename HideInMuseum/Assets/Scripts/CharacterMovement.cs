using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterMovementStateMachine _stateMachine;

    void Start()
    {
        _stateMachine.Initialize(gameObject);
        _stateMachine.ChangeState<CharacterMovementStateMove>(true);
    }

    void Update()
    {
        _stateMachine.Update();
    }
    
    public void StartPath(Vector3 position)
    {
        _stateMachine.IsMoving = false;
        _stateMachine.Path.Clear();
        _stateMachine.PositionsCount = 0;
        Color c = Color.white;
        c.a = 0.5f;
        _stateMachine.LineRenderer.SetColors(c, c);
        _stateMachine.LastLineRendererPosition = position;
        _stateMachine.WrongPath = false;
    }

    public void ContinuePath(Vector3 position)
    {
        if (_stateMachine.Path.Count > 0)
        {
            if (Vector3.Distance(position, _stateMachine.Path[_stateMachine.Path.Count - 1]) < _stateMachine.Tolerance)
            {
                return;
            }
        }
        float dist = Vector3.Distance(_stateMachine.LastLineRendererPosition, position);
        if (dist > 0.2f)
        {
            Vector3 diff = (position - _stateMachine.LastLineRendererPosition);
            int k = (int)(dist * 10.0f);
            diff *= 1.0f / k;
            for (int i = 1; i < k; ++i)
            {
                _stateMachine.LineRenderer.SetVertexCount(_stateMachine.PositionsCount + 1);
                _stateMachine.LineRenderer.SetPosition(_stateMachine.PositionsCount, _stateMachine.LastLineRendererPosition + diff * i);
                _stateMachine.LineRendererPoints.Add(_stateMachine.LastLineRendererPosition + diff * i);
                _stateMachine.PositionsCount += 1;
            }
        }
        _stateMachine.LineRenderer.SetVertexCount(_stateMachine.PositionsCount + 1);
        _stateMachine.LineRenderer.SetPosition(_stateMachine.PositionsCount, position);
        _stateMachine.LineRendererPoints.Add(position);
        _stateMachine.LastLineRendererPosition = position;
        _stateMachine.PositionsCount += 1;
        _stateMachine.Path.Add(position);
    }

    public void EndPath(Vector3 position)
    {
        if (_stateMachine.WrongPath)
        {
            _stateMachine.LineRenderer.SetColors(Color.red, Color.red);
        }
        else
        {
            _stateMachine.LineRenderer.SetColors(Color.white, Color.white);
        }
        _stateMachine.Path.Add(position);
        _stateMachine.IsMoving = true;
        _stateMachine.CurrentPathIndex = 0;
        _stateMachine.MoveTimer = 0.0f;
        _stateMachine.LastPosition = _stateMachine.Owner.transform.position;
        _stateMachine.DistanceToNextNode = Vector3.Distance(_stateMachine.Path[0], transform.position);
    }

    public void WrongPath()
    {
        _stateMachine.WrongPath = true;
        Color c = Color.red;
        c.a = 0.5f;
        _stateMachine.LineRenderer.SetColors(c, c);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        _stateMachine.ObstacleToPlayerDirection = (col.transform.position - transform.position).normalized;
        _stateMachine.ChangeState<CharacterMovementStateBlocked>();
    }
}

[Serializable]
public class CharacterMovementStateMachine : StateMachine
{
    public LineRenderer LineRenderer;
    public float Speed;
    public float Tolerance;

    [HideInInspector]
    public List<Vector3> Path = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> LineRendererPoints = new List<Vector3>();
    [HideInInspector]
    public Vector3 LastPosition;
    [HideInInspector]
    public Vector3 LastLineRendererPosition;
    [HideInInspector]
    public Vector3 ObstacleToPlayerDirection;
    [HideInInspector]
    public float MoveTimer;
    [HideInInspector]
    public float DistanceToNextNode = 0.0f;
    [HideInInspector]
    public int CurrentPathIndex = 0;
    [HideInInspector]
    public int PositionsCount = 0;
    [HideInInspector]
    public bool IsMoving;
    [HideInInspector]
    public bool WrongPath = false;
    [HideInInspector]
    public bool Blocked = false;
}

public class CharacterMovementStateMove : StateMachine.State
{
    private CharacterMovementStateMachine _characterSM;

    public override void OnEnter(StateMachine stateMachine)
    {
        _characterSM = (CharacterMovementStateMachine)stateMachine;
        _characterSM.LineRenderer.sortingOrder = 2;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (_characterSM.IsMoving)
        {
            if (_characterSM.CurrentPathIndex < _characterSM.Path.Count)
            {
                _characterSM.MoveTimer += Time.deltaTime * _characterSM.Speed / _characterSM.DistanceToNextNode;
                _characterSM.Owner.transform.position = Vector3.Lerp(_characterSM.LastPosition, _characterSM.Path[_characterSM.CurrentPathIndex], _characterSM.MoveTimer);
                UpdateLineRendererPoints();
                if (_characterSM.MoveTimer >= 1.0f)
                {
                    _characterSM.MoveTimer = 0.0f;
                    _characterSM.CurrentPathIndex += 1;
                    _characterSM.LastPosition = _characterSM.Owner.transform.position;
                    if (_characterSM.CurrentPathIndex >= _characterSM.Path.Count)
                    {
                        _characterSM.IsMoving = false;
                    }
                    else
                    {
                        _characterSM.DistanceToNextNode = Vector3.Distance(_characterSM.Path[_characterSM.CurrentPathIndex], _characterSM.LastPosition);
                    }
                }
            }
        }
    }

    void UpdateLineRendererPoints()
    {
        int index = 0;
        float closestDist = float.MaxValue;
        for (int i = 0; i < _characterSM.LineRendererPoints.Count; ++i)
        {
            float dist = Vector3.Distance(_characterSM.Owner.transform.position, _characterSM.LineRendererPoints[i]);
            if (dist < closestDist)
            {
                index = i;
                closestDist = dist;
            }
        }

        if (closestDist > 0.2f)
        {
            return;
        }

        _characterSM.LineRendererPoints = _characterSM.LineRendererPoints.GetRange(index, _characterSM.LineRendererPoints.Count - index);

        _characterSM.LineRenderer.SetVertexCount(_characterSM.LineRendererPoints.Count);
        _characterSM.LineRenderer.SetPositions(_characterSM.LineRendererPoints.ToArray());
    }
}

public class CharacterMovementStateBlocked : StateMachine.State
{
    private CharacterMovementStateMachine _characterSM;

    private Vector3 _initPosition;
    private float _timer;

    public override void OnEnter(StateMachine stateMachine)
    {
        _characterSM = (CharacterMovementStateMachine)stateMachine;
        _characterSM.IsMoving = false;
        _characterSM.Path.Clear();
        _characterSM.LineRenderer.SetVertexCount(0);
        _timer = 0.0f;
        _initPosition = _characterSM.Owner.transform.position;
        _characterSM.Blocked = true;
    }

    public override void OnExit()
    {
        _characterSM.LastPosition = _characterSM.Owner.transform.position;
        _characterSM.Blocked = false;
    }

    public override void Update()
    {
        _timer += 10.0f * Time.deltaTime;
        _characterSM.Owner.transform.position = _initPosition + new Vector3(Mathf.Sin(_timer), Mathf.Cos(_timer)) * 0.05f;
        if(_timer > 15f)
        {
            _characterSM.Owner.transform.position = _initPosition - _characterSM.ObstacleToPlayerDirection * 0.05f;
            _characterSM.ChangeState<CharacterMovementStateMove>();
        }
    }
}