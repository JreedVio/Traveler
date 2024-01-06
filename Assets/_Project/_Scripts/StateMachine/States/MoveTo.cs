using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private readonly float _slowestSpeed;
    private readonly float _initialRotationSpeed;
    private readonly AnimationCurve _stoppingCurve;
    private readonly AnimationCurve _turningCurve;
    
    public bool IsDestinationReached = false;

    private bool _isFollowingPath = false;
    private int _pathIndex = 0;
    private Path _path;
    
    private Quaternion _targetRotation;
    private Quaternion _initialRotation;
    private float _tRotation = 0f;


    public MoveTo(Transform object_, Vector3 destination, MovementParameters movementParameters)
    {
        _object = object_;
        _destination = destination;
        _speed = movementParameters.Speed;
        _turnDistance = movementParameters.TurnDistance;
        _turnSpeed = movementParameters.TurnSpeed;
        _stoppingDistance = movementParameters.StoppingDistance;
        _stoppingCurve = movementParameters.StoppingCurve;
        _slowestSpeed = movementParameters.SlowestSpeed;
        _initialRotationSpeed = movementParameters.InitialRotationSpeed;
        _turningCurve = movementParameters.TurningCurve;
    }
    
    // Handle movement
    public void Tick()
    {
        if (_path != null && !_isFollowingPath)
        {
            _tRotation += _initialRotationSpeed * Time.deltaTime;
            _object.transform.rotation = Quaternion.Lerp(_initialRotation, _targetRotation, _turningCurve.Evaluate(_tRotation));
            if (_tRotation >= 1f)
            {
                _object.GetComponent<Animator>().SetFloat("Speed", 1f);
                _isFollowingPath = true;
            }
        }

        if (!_isFollowingPath || IsDestinationReached) return;
        
        Vector2 pos2D = new Vector2(_object.position.x, _object.position.z);
        // Check if the object is past the current index
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

        // Move and rotate the object
        if (_isFollowingPath)
        {
            // Slow down before the finish point
            float currentSpeed = _speed;
            float distanceToFinish = _path.turnBoundaries[_path.finishLineIndex].DistanceFromPoint(pos2D);
            if (distanceToFinish <= _stoppingDistance && _pathIndex >= _path.slowDownIndex && _stoppingDistance > 0)
                currentSpeed = Mathf.Lerp(_speed, _slowestSpeed, _stoppingCurve.Evaluate(1 - distanceToFinish / _stoppingDistance));
            
            Quaternion targetRotation = Quaternion.LookRotation(_path.lookPoints[_pathIndex] - new Vector3(_object.transform.position.x, 0, _object.transform.position.z));
            _object.transform.rotation = Quaternion.Lerp(_object.transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
            _object.transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed, Space.Self);
        }
        
    }

    public void OnEnter()
    {
        PathRequestManager.RequestPath(_object.position, _destination, OnPathFound);
    }
    
    public void OnExit()
    {
        // Reset all variables to default
        _isFollowingPath = false;
        _path = null;
        _pathIndex = 0;
        IsDestinationReached = false;
        _targetRotation = Quaternion.identity;
        _initialRotation = Quaternion.identity;
        _tRotation = 0f;

        _object.GetComponent<Animator>().SetFloat("Speed", 0f);
    }
    
    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = new Path(waypoints, _object.position, _turnDistance, _stoppingDistance);
            _pathIndex = 0;
            _targetRotation = Quaternion.LookRotation(_path.lookPoints[0] - new Vector3(_object.transform.position.x, 0, _object.transform.position.z));
            _initialRotation = _object.transform.rotation;
            _tRotation = 0f;
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