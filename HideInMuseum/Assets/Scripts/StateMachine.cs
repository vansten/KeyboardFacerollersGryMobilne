using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
    #region Nested class State

    public abstract class State
    {
        public abstract void OnEnter(StateMachine stateMachine);
        public abstract void OnExit();
        public abstract void Update();
    }

    #endregion

    [HideInInspector]
    public GameObject Owner;

    private List<State> _states;
    private int _currentStateIndex = 0;

    public void Initialize(GameObject owner)
    {
        Owner = owner;
        _states = new List<State>();
    }

    public void Update()
    {
        if(_states != null && _states.Count > 0 && _states[_currentStateIndex] != null)
        {
            _states[_currentStateIndex].Update();
        }
    }

    public void ChangeState<T>(bool force = false) where T : State, new()
    {
        if (_states.Count > 0)
        {
            if(_states[_currentStateIndex].GetType() == typeof(T))
            {
                if(!force)
                {
                    return;
                }
            }

            foreach (State s in _states)
            {
                if (s.GetType() == typeof(T))
                {
                    if(_states.Count > _currentStateIndex && _states[_currentStateIndex] != null)
                    {
                        _states[_currentStateIndex].OnExit();
                    }
                    s.OnEnter(this);
                    _currentStateIndex = _states.IndexOf(s);
                    return;
                }
            }
        }
        
        if (_states.Count > _currentStateIndex && _states[_currentStateIndex] != null)
        {
            _states[_currentStateIndex].OnExit();
        }
        State newState = new T();
        newState.OnEnter(this);
        _states.Add(newState);
        _currentStateIndex = _states.IndexOf(newState);
    }
}
