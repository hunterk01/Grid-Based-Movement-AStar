using UnityEngine;
using System.Collections.Generic;

public class GridBase : MonoBehaviour
{
    [Header("Grid Creation")]
    private GridNode[,] grid;
    public bool showGrid;
    public Vector2 gridSize;
    [HideInInspector] public int gridSizeX, gridSizeY;
    public float nodeRadius;
    private float nodeDiameter;

    [Header("Navigation Values")]
    public LayerMask unwalkableMask;
    public GameObject visualNodeObject;

    // Node and selection tile lists
    private List<GridNode> nodeList = new List<GridNode>();
    [HideInInspector] public List<GameObject> selectableObjects = new List<GameObject>();

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        BuildGrid();
    }

    public int MaxNodes { get { return gridSizeX * gridSizeY; } }

    private void BuildGrid()
    {
        grid = new GridNode[gridSizeX, gridSizeY];
        Vector3 lowerLeftWorldPosition = new Vector3(transform.position.x - gridSizeX / 2, transform.position.y, transform.position.z - gridSizeY / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Work from the lower left corner of the grid and set the offsets to the center of each node position
                Vector3 nodeShiftX = Vector3.right * (x + nodeRadius);
                Vector3 nodeShiftY = Vector3.forward * (y + nodeRadius);
                Vector3 nodePoint = lowerLeftWorldPosition + nodeShiftX + nodeShiftY;

                // Check if the node position contains unwalkable objects and add the node to the grid
                bool walkable = !(Physics.CheckSphere(nodePoint, nodeRadius, unwalkableMask));
                grid[x, y] = new GridNode(nodePoint, x, y, walkable);
                nodeList.Add(grid[x, y]);
            }
        }
    }

    public void CreateSelectableNode(Vector3 position)
    {
        GameObject obj = Instantiate(visualNodeObject, position, Quaternion.identity);
        selectableObjects.Add(obj);
    }

    public void DestroySelectableObjects()
    {
        foreach (GameObject obj in selectableObjects)
        {
            Destroy(obj);
        }
    }

    public void ClearNodeCosts()
    {
        foreach (GridNode node in nodeList)
        {
            node.gCost = 0;
            node.hCost = 0;
        }
    }

    public GridNode GetNodeFromPosition(Vector3 worldPosition)
    {
        // Get percentage of how far along the grid the requestors position is
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Round results to int, so they are in the center of the node positions
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the nodes own position
                if ((x == 0 && y == 0) || (x != 0 && y != 0))
                {
                    continue;
                }

                // Check if the position being checked is inside of the grid
                int checkX = node.gridPositionX + x;
                int checkY = node.gridPositionY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    // If so, add the node to the neighbors list
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    private void OnDrawGizmos()
    {
        if (showGrid && grid != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

            foreach (GridNode node in grid)
            {
                Gizmos.color = (node.isWalkable ? Color.green : Color.red);
                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
