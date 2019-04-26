using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask selectableMask;
    GameObject focusedObject;
    GameObject lastObject;
    GameObject selectedObject;

    UnitMovement unitMovement;

    public enum PlayerState { Idle, Selection, Moving }
    [HideInInspector] public PlayerState playerState;

    private void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        playerState = PlayerState.Idle;
    }

    void Update()
    {
        CheckPlayerState();

        GetObjectAtMousePosition();

        if (Input.GetMouseButtonDown(0))
            SelectObjectOnMouseClick();
    }

    private void CheckPlayerState()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                SetPlayerIdle();
                break;
            case PlayerState.Selection:
                EnablePlayerSelection();
                break;
            case PlayerState.Moving:
                // An empty state for while the player is moving to new node
                break;
        }
    }

    private void SetPlayerIdle()
    {
        unitMovement.anim.SetBool("Walking", false);
        playerState = PlayerState.Selection;
    }

    private void EnablePlayerSelection()
    {
        if (!unitMovement.selectableNodesSetup)
        {
            unitMovement.CreateSelectableNodes();
        }
    }

    // Highlights the selectable node at the mouse position
    private void GetObjectAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, selectableMask))
        {
            focusedObject = hit.transform.gameObject;
        }

        if (focusedObject != lastObject)
        {
            focusedObject.GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
            if (lastObject != null)
                lastObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;

            lastObject = focusedObject;

            unitMovement.SubmitCostRequest(hit.transform.position);
        }
    }

    // Selects the clicked node and submits a move request for the player
    private void SelectObjectOnMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, selectableMask))
        {
            selectedObject = hit.transform.gameObject;
            selectedObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            unitMovement.SumbitMoveRequest(hit.transform.position);
            unitMovement.ResetSelectableNodes();

            // Set playerState to moving, to disable selectable node creation
            playerState = PlayerState.Moving;
            unitMovement.selectableNodesSetup = false;
        }
    }
}
