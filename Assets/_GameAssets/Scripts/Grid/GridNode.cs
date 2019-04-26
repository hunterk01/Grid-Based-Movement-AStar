using UnityEngine;

public class GridNode : IHeapItem<GridNode>
{
    public bool isWalkable;
    public Vector3 worldPosition;

    public int gCost, hCost;
    public int fCost { get { return gCost + hCost; } }

    public int gridPositionX, gridPositionY;
    public GridNode parent;
    int heapIndex;

    public GridNode(Vector3 _worldPosition, int _gridPositionX, int _gridPositionY, bool _isWalkable)
    {
        worldPosition = _worldPosition;
        gridPositionX = _gridPositionX;
        gridPositionY = _gridPositionY;
        isWalkable = _isWalkable;
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(GridNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
