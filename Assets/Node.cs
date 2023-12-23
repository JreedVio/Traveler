using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{

    public bool Walkable;
    public Vector3 WorldPosition;
    public int GridX;
    public int GridY;

    public int GCost;
    public int HCost;
    public Node Parent;
    private int _heapIndex;

    public Node(bool walkable_, Vector3 worldPos_, int gridX_, int gridY_)
    {
        Walkable = walkable_;
        WorldPosition = worldPos_;
        GridX = gridX_;
        GridY = gridY_;
    }

    public int FCost => GCost + HCost;

    public int HeapIndex
    {
        get => _heapIndex;
        set => _heapIndex = value;
    }

    public int CompareTo(Node nodeToCompare_)
    {
        int compare = FCost.CompareTo(nodeToCompare_.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare_.HCost);
        }

        return -compare;
    }
}
