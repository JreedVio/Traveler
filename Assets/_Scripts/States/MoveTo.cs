using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MoveTo : IState
{
    private readonly Transform _object;
    private readonly Vector3 _destination;
    private readonly float _speed;
    private readonly float _turnDistance;
    private readonly float _turnSpeed;
    private readonly float _stoppingDistance;
    private readonly AnimationCurve _stoppingCurve = AnimationCurve.Linear(0, 0, 1, 1);
    private readonly float _slowestSpeed;

    
    public bool IsDestinationReached = false;

    private bool _isFollowingPath = false;
    private int _pathIndex = 0;
    private Path _path;


    public MoveTo(Transform object_, Vector3 destination, float speed, 
        float turnDistance, float turnSpeed, float stoppingDistance, float slowestSpeed,
        AnimationCurve stoppingCurve = null)
    {
        _object = object_;
        _destination = destination;
        _speed = speed;
        _turnDistance = turnDistance;
        _turnSpeed = turnSpeed;
        _stoppingDistance = stoppingDistance;
        if(stoppingCurve != null) _stoppingCurve = stoppingCurve;
        _slowestSpeed = slowestSpeed;
    }
    
    public void Tick()
    {
        if (!_isFollowingPath || IsDestinationReached) return;
        
        Vector2 pos2D = new Vector2(_object.position.x, _object.position.z);
        while (_path.turnBoundaries[_pathIndex].HasCrossedLine(pos2D))
        {
            if (_pathIndex == _path.finishLineIndex)
            {
                _isFollowingPath = false;
                IsDestinationReached = true;
                break;
            }
            else
                _pathIndex++;
        }

        if (_isFollowingPath)
        {
            // Slow down before the finish point
            float currentSpeed = _speed;
            float distanceToFinish = _path.turnBoundaries[_path.finishLineIndex].DistanceFromPoint(pos2D);
            if (distanceToFinish <= _stoppingDistance && _pathIndex >= _path.slowDownIndex && _stoppingDistance > 0)
                currentSpeed = Mathf.Lerp(currentSpeed, _slowestSpeed, _stoppingCurve.Evaluate(1 - distanceToFinish / _stoppingDistance));
            
            Quaternion targetRotation = Quaternion.LookRotation(_path.lookPoints[_pathIndex] - _object.transform.position);
            _object.transform.rotation = Quaternion.Lerp(_object.transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
            _object.transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed, Space.Self);
        }
        
    }

    public void OnEnter()
    {
        PathRequestManager.RequestPath(_object.position, _destination, OnPathFound);
        _object.GetComponent<Animator>().SetFloat("Speed", 1f);
    }
    
    public void OnExit()
    {
        // Reset all variables to default
        _isFollowingPath = false;
        _path = null;
        _pathIndex = 0;
        IsDestinationReached = false;

        _object.GetComponent<Animator>().SetFloat("Speed", 0f);
    }
    
    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _isFollowingPath = true;
            _path = new Path(waypoints, _object.position, _turnDistance, _stoppingDistance);
            _pathIndex = 0;
            _object.transform.LookAt(_path.lookPoints[0]);
        }
        else
        {
            Debug.LogError("Error in finding a path");
        }
    }

    public void OnDrawGizmos()
    {
        if(_path != null)
            _path.DrawWithGizmos();
    }
}
