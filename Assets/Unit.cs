using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class Unit : MonoBehaviour
{
    public Transform Target;
    public float Speed = 30;
    private Vector3[] _path;
    private int _targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
    }

    private void OnPathFound(Vector3[] newPath_, bool pathSuccessful_)
    {
        if (pathSuccessful_)
        {
            _path = newPath_;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = _path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                _targetIndex++;
                if(_targetIndex >= _path.Length){
                    _targetIndex = 0;
                    _path = new Vector3[0];
                    yield break;
                }
                currentWaypoint = _path[_targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(_path[i], Vector3.one);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i-1], _path[i]);
                }
            }
        }
    }
}
