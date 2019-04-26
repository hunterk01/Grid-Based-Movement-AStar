using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private GridBase gridBase;
    private PlayerController playerControl;

    // Player movement and animation
    private float moveSpeed = 3;
    private float rotationSpeed = 8;
    private int movementPoints = 6;
    [HideInInspector] public Animator anim;

    // Pathfinding
    private Vector3[] path;
    private int targetIndex;

    // Variables for showing selectable movement tiles
    private Vector3 nodePosition;
    private GridNode n;
    [HideInInspector] public float costToNode = 0;
    [HideInInspector] public bool selectableNodesSetup = false;

    void Start()
    {
        gridBase = GameObject.FindObjectOfType<GridBase>().GetComponent<GridBase>();
        playerControl = GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
    }

    // This is used to get node costs for setting up selectable nodes
    public void SubmitCostRequest(Vector3 targetPosition)
    {
        gridBase.ClearNodeCosts();
        PathRequestManager.RequestPath(transform.position, targetPosition, OnCostFound);
    }

    public void SumbitMoveRequest(Vector3 targetPosition)
    {
        gridBase.ClearNodeCosts();
        PathRequestManager.RequestPath(transform.position, targetPosition, OnPathFound);
    }

    public void CreateSelectableNodes()
    {
        for (int x = -movementPoints; x <= movementPoints; x++)
        {
            for (int z = -movementPoints; z <= movementPoints; z++)
            {
                // Get node position and cost
                Vector3 nodePosition = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
                SubmitCostRequest(nodePosition);

                // Skip the node the player is on
                if (x == 0 && z == 0)
                    continue;

                // Check if the node position is inside of the grid
                if (nodePosition.x >= -gridBase.gridSizeX / 2 && nodePosition.x <= gridBase.gridSizeX / 2 &&
                   nodePosition.z >= -gridBase.gridSizeY / 2 && nodePosition.z <= gridBase.gridSizeY / 2)
                {
                    // If the node is within the player movement range and is walkable, create a selectable tile
                    GridNode n = gridBase.GetNodeFromPosition(nodePosition);
                    if (costToNode <= movementPoints * 10 && n.isWalkable)
                    {
                        gridBase.CreateSelectableNode(nodePosition);
                    }
                }
            }
        }
        selectableNodesSetup = true;
    }

    // Used to clear the selectable tiles once the player has chosen one
    public void ResetSelectableNodes()
    {
        gridBase.DestroySelectableObjects();
    }

    public void OnCostFound(Vector3[] newPath, bool pathSuccessful, float pathCost)
    {
        if (pathSuccessful)
        {
            costToNode = pathCost;
        }
    }

    // When the lowest cost path is found, call the FollowPath coroutine
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful, float pathCost)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            // Check if the player has reached the next waypoint
            if (transform.position == currentWaypoint)
            {
                targetIndex++;

                // If the player has reached the last waypoint, turn off the walking animation and exit the coroutine
                if (targetIndex >= path.Length)
                {
                    playerControl.playerState = PlayerController.PlayerState.Idle;
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            // Set the player rotation, direction, and walk animation and move the player to the next waypoint
            Vector3 targetDirection = currentWaypoint - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newDirection);
            anim.SetBool("Walking", true);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        // Draws the waypoints and path in the scene view
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(path[i], 0.2f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
