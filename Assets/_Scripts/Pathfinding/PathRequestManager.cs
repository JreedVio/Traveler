using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> _pathRequestQueue;
    private PathRequest _currentPathRequest;

    private static PathRequestManager _instance;
    private Pathfinding _pathfinding;

    private bool _isProcessingPath;

    private void Awake()
    {
        _instance = this;
        _pathfinding = GetComponent<Pathfinding>();
        _pathRequestQueue = new Queue<PathRequest>();
    }

    public static void RequestPath(Vector3 pathStart_, Vector3 pathEnd_, Action<Vector3[], bool> callback_)
    {
        PathRequest newRequest = new PathRequest(pathStart_, pathEnd_, callback_);
        _instance._pathRequestQueue.Enqueue(newRequest);
        _instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (!_isProcessingPath && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessingPath = true;
            _pathfinding.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path_, bool success_)
    {
        _currentPathRequest.Callback(path_, success_);
        _isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> Callback;
        public PathRequest(Vector3 start_, Vector3 end_, Action<Vector3[], bool> callback_)
        {
            PathStart = start_;
            PathEnd = end_;
            Callback = callback_;
        }
    }
}
