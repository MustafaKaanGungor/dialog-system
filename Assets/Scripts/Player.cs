using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    private Vector2 movementInput;
    [SerializeField] private float moveSpeed = 5f;
    private bool isWalking;
    [SerializeField] private bool isInteracting;
    private Vector3 lastInteractDir;
    [SerializeField] private LayerMask interactableLayerMask;
    private NPC selectedNPC;
    private NPC interactedNPC;
    private float interactionDistance = 3f;
    public Action<NPC> NPCInteracted;
    public Action<NPC> StoppedInteraction;


    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        } else {
            Instance = this;
        }
    }

    private void Start() {
        GameInput.Instance.InteractPerformed += OnInteractAction;
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();

        if(isInteracting)
        {
            if(Vector3.Distance(transform.position, interactedNPC.transform.position) > interactionDistance + 0.5f)
            {
                StopInteraction();    
            }
                
        }
    }

    private void HandleMovement()
    {
        movementInput = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirection, moveDistance);

        if(!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0f, 0f).normalized;
            canMove = moveDirX.x != 0f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirX, moveDistance);

            if(canMove)
            {
                moveDirection = moveDirX;
            } else
            {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDirection.z).normalized;
                canMove = moveDirZ.z != 0f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirZ, moveDistance);
                if(canMove)
                {
                    moveDirection = moveDirZ;
                }
            }
        }
        
        if(canMove)
        {

            transform.position += moveDirection * moveDistance;
        }

        isWalking = moveDirection != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.1f);
    }

    private void HandleInteraction()
    {
        movementInput = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);

        if(moveDirection != Vector3.zero)
        {
            lastInteractDir = moveDirection;
        }

        Vector3 rayCastOrigin = transform.position + Vector3.up;
        Physics.Raycast(rayCastOrigin, lastInteractDir, out RaycastHit hit, interactionDistance, interactableLayerMask);

        if(hit.collider)
        {
            if(hit.collider.TryGetComponent(out NPC npc))
            {
                if(npc != selectedNPC)
                {
                    SetSelectedNPC(npc);
                    Debug.Log("Selected NPC: " + npc.gameObject.name);
                }
            }
            else
            {
                SetSelectedNPC(null);
            }
        } else
        {
            SetSelectedNPC(null);
        }
    }

    private void SetSelectedNPC(NPC npc)
    {
        if(selectedNPC != null)
        {
            selectedNPC.GetComponentInChildren<Outline>().enabled = false;
        }
        selectedNPC = npc;
        if(npc != null) {
            npc.GetComponentInChildren<Outline>().enabled = true;
        }
    }

    private void OnInteractAction()
    {
        Debug.Log("Interact action performed");
        if(selectedNPC != null)
        {
            selectedNPC.Interact();
            NPCInteracted?.Invoke(selectedNPC);
            isInteracting = true;
            interactedNPC = selectedNPC;
        }
    }

    private void StopInteraction()
    {
        isInteracting = false;
        StoppedInteraction?.Invoke(interactedNPC);
        SetSelectedNPC(null);
        interactedNPC = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayCastOrigin = transform.position + Vector3.up;
        Gizmos.DrawLine(rayCastOrigin, rayCastOrigin + lastInteractDir * 1.5f);
    }
}
