using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count { get { return currentItemCount; } }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            // Set the parent item and compare it's fCost to the current item's fCost
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                // If it's lower, swap the items
                SwapItems(item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // Check if a left child exists
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                // Check if a righ child exists
                if (childIndexRight < currentItemCount)
                {
                    // Check if the right child is less than the left child
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        // If so, change the swap index to the right child
                        swapIndex = childIndexRight;
                    }
                }

                // Check if the swapIndex item is less than the parent item.  If so, swap them
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    SwapItems(item, items[swapIndex]);
                }
                else
                {
                    return; // The item is in it's correct position
                }
            }
            else
            {
                return;  // The item is in it's correct position
            }
        }
    }

    private void SwapItems(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int tempItemIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempItemIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
