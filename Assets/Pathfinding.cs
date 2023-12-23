using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{
    private PathRequestManager _requestManager;
    
    private Grid _grid;
    private Heap<Node> _openSet;
    private HashSet<Node> _closedSet;
    
    private void Awake()
    {
        _requestManager = GetComponent<PathRequestManager>();
        _grid = GetComponent<Grid>();
    }
    public void StartFindPath(Vector3 startPos_, Vector3 targetPos_)
    {
        _openSet = new Heap<Node>(_grid.MaxSize);
        _closedSet = new HashSet<Node>();
        StartCoroutine(FindPath(startPos_, targetPos_));
    }

    IEnumerator FindPath(Vector3 startPos_, Vector3 targetPos_)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        
        Node startNode = _grid.NodeFromWorldPoint(startPos_);
        Node targetNode = _grid.NodeFromWorldPoint(targetPos_);

        if (startNode.Walkable && targetNode.Walkable)
        {
            _openSet.Clear();
            _closedSet.Clear();
            _openSet.Add(startNode);

            while (_openSet.Count > 0)
            {
                Node currentNode = _openSet.RemoveFirst();
                _closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || _closedSet.Contains(neighbour)) continue;
                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !_openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!_openSet.Contains(neighbour))
                            _openSet.Add(neighbour);
                        else
                        {
                            _openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;
        if(pathSuccess) waypoints = RetracePath(startNode, targetNode);
        _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode_, Node endNode_)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode_;
        while (currentNode != startNode_)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Add(currentNode);

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path_)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path_.Count; i++)
        {
            Vector2 directionNew = 
                new Vector2(path_[i - 1].GridX - path_[i].GridX, path_[i - 1].GridY - path_[i].GridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path_[i].WorldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA_, Node nodeB_)
    {
        int dstX = Mathf.Abs(nodeA_.GridX - nodeB_.GridX);
        int dstY = Mathf.Abs(nodeA_.GridY - nodeB_.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
