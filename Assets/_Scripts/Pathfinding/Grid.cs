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
    [SerializeField] private int _obstacleProximityPenalty = 10;
    
    private LayerMask _walkableMask;
    private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();

    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    private int _penaltyMin = 0;
    private int _penaltyMax = 0;
    
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

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, _walkableMask))
                    _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                
                if(!walkable)
                    movementPenalty += _obstacleProximityPenalty;
                
                _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
    }

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = blurSize;

        int[,] penaltiesHorizontalPath = new int[_gridSizeX, _gridSizeY];
        int[,] penaltiesVerticalPath = new int[_gridSizeX, _gridSizeY];

        for (int y = 0; y < _gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPath[0, y] += _grid[sampleX, y].MovementPenalty;
            }

            for (int x = 1; x < _gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);
                
                penaltiesHorizontalPath[x, y] = penaltiesHorizontalPath[x - 1, y] - _grid[removeIndex, y].MovementPenalty +
                                                _grid[addIndex, y].MovementPenalty;
            }
        }
        
        
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPath[x, 0] += penaltiesHorizontalPath[x, sampleY];
            }
            
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPath[x, 0] / (kernelSize * kernelSize));
            _grid[x, 0].MovementPenalty = blurredPenalty;

            for (int y = 1; y < _gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);
                
                penaltiesVerticalPath[x, y] = penaltiesVerticalPath[x, y - 1] - penaltiesHorizontalPath[x, removeIndex] +
                                              penaltiesHorizontalPath[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPath[x, y] / (kernelSize * kernelSize));
                _grid[x, y].MovementPenalty = blurredPenalty;
                
                if(blurredPenalty > _penaltyMax) _penaltyMax = blurredPenalty;
                if(blurredPenalty < _penaltyMin) _penaltyMin = blurredPenalty;
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
                if(!n.Walkable) Gizmos.color = Color.red;
                else Gizmos.color = Color.Lerp(Color.white, Color.black, 
                    Mathf.InverseLerp(_penaltyMin, _penaltyMax, n.MovementPenalty));
                
                Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter));
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
