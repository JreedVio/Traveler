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