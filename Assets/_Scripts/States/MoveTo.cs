using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : IState
{
    private readonly Transform _object;
    private readonly Vector3 _destination;
    private readonly float _speed;
    
    public float TimeWaited = 0f;
    public bool IsDestinationReached = false;
    
    private bool _isPathFound = false;
    private Vector3[] _path;
    private int _targetIndex = 0;
    private Vector3 _currentWaypoint = Vector3.zero;


    public MoveTo(Transform object_, Vector3 destination_, float speed_)
    {
        _object = object_;
        _destination = destination_;
        _speed = speed_;
    }
    public void Tick()
    {
        if (!_isPathFound) return;
        
        if (_object.position == _currentWaypoint)
        {
            _targetIndex++;
            if(_targetIndex >= _path.Length){
                TimeWaited += Time.deltaTime;
                IsDestinationReached = true;
                return;
            }
            _currentWaypoint = _path[_targetIndex];
        }
        else
        {
            _object.forward = (_currentWaypoint - _object.position).normalized;
            _object.position = Vector3.MoveTowards(_object.position, _currentWaypoint, _speed * Time.deltaTime);
        }
    }

    public void OnEnter()
    {
        PathRequestManager.RequestPath(_object.position, _destination, OnPathFound);
    }
    
    private void OnPathFound(Vector3[] newPath_, bool pathSuccessful_)
    {
        if (pathSuccessful_)
        {
            _isPathFound = pathSuccessful_;
            _path = newPath_;
            _currentWaypoint = _path[0];
        }
        else
        {
            Debug.LogError("Error in finding a path");
        }
    }

    public void OnExit()
    {
        TimeWaited = 0f;
        _isPathFound = false;
        _targetIndex = 0;
        _path = new Vector3[0];
        _currentWaypoint = Vector3.zero;
        IsDestinationReached = false;
    }
    
}
