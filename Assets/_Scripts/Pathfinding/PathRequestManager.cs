/*
Copyright (c) 2017 Sebastian Lague

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
