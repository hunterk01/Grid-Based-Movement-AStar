using UnityEngine;
using System;
using System.Collections.Generic;

public class GridPathfinder : MonoBehaviour
{
    GridBase grid;
    PathRequestManager pathRequestManager;
    float pathCost = 0;

    private void Awake()
    {
        grid = GetComponent<GridBase>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void BeginFindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        FindPath(startPosition, targetPosition);
    }

    public void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // Convert the start and target world positions to nodes
        GridNode startNode = grid.GetNodeFromPosition(startPosition);
        GridNode targetNode = grid.GetNodeFromPosition(targetPosition);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            // Create open and closed sets, then add the current node to the open set
            Heap<GridNode> openSet = new Heap<GridNode>(grid.MaxNodes);
            HashSet<GridNode> closedSet = new HashSet<GridNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Set the currentNode, then move it from the openSet to the closedSet
                GridNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                // Check if the currentNode is the targetNode
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (GridNode neighbor in grid.GetNeighbors(currentNode))
                {
                    // Check if the neighbor is not walkable, or is already in the closed set
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    // Check new movement cost from the current node to the neighbor
                    int movementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                    if (movementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        // Set the new neighbors costs and parent node
                        neighbor.gCost = movementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        // Then add it to the openSet
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            pathCost = targetNode.gCost;
            waypoints = RetracePath(startNode, targetNode);
        }
        pathRequestManager.FinishedProcessingPath(waypoints, pathSuccess, pathCost);
    }

    private Vector3[] RetracePath(GridNode startNode, GridNode targetNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(currentNode); // Add the startPosition to the list

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    private Vector3[] SimplifyPath(List<GridNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        List<Vector3> simplifiedWaypoints = new List<Vector3>();
        Vector3 oldDirection = Vector3.zero;

        for (int i = 1; i < path.Count; i++)
        {
            waypoints.Add(path[i].worldPosition);
        }

        simplifiedWaypoints.Add(path[0].worldPosition); // Add the target waypoint to the list;

        for (int i = 1; i < waypoints.Count; i++)
        {
            Vector3 newDirection = waypoints[i - 1] - waypoints[i];
            if (newDirection != oldDirection)
            {
                simplifiedWaypoints.Add(path[i].worldPosition);
            }
            oldDirection = newDirection;
        }
        return simplifiedWaypoints.ToArray();
    }

    private int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        // Get the node distances along the X and Y axis
        int distanceX = Mathf.Abs(nodeA.gridPositionX - nodeB.gridPositionX);
        int distanceY = Mathf.Abs(nodeA.gridPositionY - nodeB.gridPositionY);

        //Check which distance is greater and return the appropriate value
        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
