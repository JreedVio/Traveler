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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{

    public bool Walkable;
    public Vector3 WorldPosition;
    public int GridX;
    public int GridY;
    public int MovementPenalty;

    public int GCost;
    public int HCost;
    public Node Parent;
    private int _heapIndex;

    public Node(bool walkable_, Vector3 worldPos_, int gridX_, int gridY_, int movementPenalty_)
    {
        Walkable = walkable_;
        WorldPosition = worldPos_;
        GridX = gridX_;
        GridY = gridY_;
        MovementPenalty = movementPenalty_;
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
