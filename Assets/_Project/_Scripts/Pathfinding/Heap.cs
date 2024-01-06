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

using UnityEngine;
using System.Collections;
using System;

public class Heap<T> where T : IHeapItem<T> {
	
    T[] _items;
    int _currentItemCount;
	
    public Heap(int maxHeapSize_) {
        _items = new T[maxHeapSize_];
        _currentItemCount = 0;
    }
	
    public void Add(T item_) {
        item_.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item_;
        SortUp(item_);
        _currentItemCount++;
    }

    public T RemoveFirst() {
        T firstItem = _items[0];
        _currentItemCount--;
        _items[0] = _items[_currentItemCount];
        _items[0].HeapIndex = 0;
        SortDown(_items[0]);
        return firstItem;
    }

    public void UpdateItem(T item_) {
        SortUp(item_);
    }
    
    public void Clear()
    { 
        _currentItemCount = 0;
    }

    public int Count => _currentItemCount;

    public bool Contains(T item_)
    {
        if (item_.HeapIndex < _currentItemCount)
        {
            return Equals(_items[item_.HeapIndex], item_);
        }
        return false;
    }

    void SortDown(T item_) {
        while (true) {
            int childIndexLeft = item_.HeapIndex * 2 + 1;
            int childIndexRight = item_.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < _currentItemCount) {
                swapIndex = childIndexLeft;

                if (childIndexRight < _currentItemCount) {
                    if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0) {
                        swapIndex = childIndexRight;
                    }
                }

                if (item_.CompareTo(_items[swapIndex]) < 0) {
                    Swap (item_,_items[swapIndex]);
                }
                else {
                    return;
                }

            }
            else {
                return;
            }

        }
    }
	
    void SortUp(T item_) {
        int parentIndex = (item_.HeapIndex-1)/2;
		
        while (true) {
            T parentItem = _items[parentIndex];
            if (item_.CompareTo(parentItem) > 0) {
                Swap (item_,parentItem);
            }
            else {
                break;
            }

            parentIndex = (item_.HeapIndex-1)/2;
        }
    }
	
    void Swap(T itemA_, T itemB_) {
        _items[itemA_.HeapIndex] = itemB_;
        _items[itemB_.HeapIndex] = itemA_;
        int itemAIndex = itemA_.HeapIndex;
        itemA_.HeapIndex = itemB_.HeapIndex;
        itemB_.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex {
        get;
        set;
    }
}