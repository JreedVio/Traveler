using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class Grid : MonoBehaviour
{
    public bool DisplayGridGizmos;
    [SerializeField] private LayerMask _unwalkableMask;
    [SerializeField] private Vector2 _gridWorldSize;
    [SerializeField] private float _nodeRadius;
    [SerializeField] private Node[,] _grid;
    [SerializeField] private TerrainType[] _walkableRegions;
    private LayerMask _walkableMask;
    private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();

    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    
    public int MaxSize => _gridSizeX * _gridSizeY;

    private void Awake()
    {
        _nodeDiameter = _nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);

        foreach (TerrainType region in _walkableRegions)
        {
            _walkableMask.value |= region.TerrainMask.value;
            _walkableRegionsDictionary.Add((int)MathF.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
        }
        
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 -
                                  Vector3.forward * _gridWorldSize.y / 2;
        
        for (int x = 0; x < _gridSizeX; x++) 
        {
            for (int y = 0; y < _gridSizeY; y++) 
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + _nodeRadius) +
                                     Vector3.forward * (y * _nodeDiameter + _nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, _nodeRadius, _unwalkableMask);

                int movementPenalty = 0;

                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, _walkableMask))
                    {
                        _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                    Debug.Log(movementPenalty);
                }

                _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
    }

    public List<Node> GetNeighbours(Node node_)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int checkX = node_.GridX + x;
                int checkY = node_.GridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition_)
    {
        float percentX = worldPosition_.x / _gridWorldSize.x + 0.5f;
        float percentY = worldPosition_.z / _gridWorldSize.y + 0.5f;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt(Mathf.Clamp((_gridSizeX) * percentX, 0, _gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Clamp((_gridSizeY) * percentY, 0, _gridSizeY - 1));

        return _grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
        if (_grid != null && DisplayGridGizmos)
        {
            foreach (Node n in _grid) 
            {
                Gizmos.color = (n.Walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter - .1f));
            }
        }
    }

    [Serializable]
    public class TerrainType
    {
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}
