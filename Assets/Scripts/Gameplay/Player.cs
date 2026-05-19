using System;
using UnityEngine;
using DialogSystem.Core;

namespace DialogSystem.Gameplay
{
    public class Player : MonoBehaviour
{
    private Vector2 movementInput;
    [SerializeField] private float moveSpeed = 5f;
    private bool isWalking;
    private bool isInteracting;
    private Vector3 lastInteractDir;
    [SerializeField] private LayerMask interactableLayerMask;
    private IInteractable selectedInteractable;
    private IInteractable interactedInteractable;
    [SerializeField] private float interactionDistance = 3f;
    private Animator animator;
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    [SerializeField] private DialogueEventChannelSO dialogueEventChannel;


    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        dialogueEventChannel.OnStoppedInteraction += OnDialogueEnded;
    }

    private void OnDisable()
    {
        dialogueEventChannel.OnStoppedInteraction -= OnDialogueEnded;
        GameInput.Instance.InteractPerformed -= OnInteractAction;
    }

    private void Start() {
        GameInput.Instance.InteractPerformed += OnInteractAction;
    }

    void Update()
    {
        if(!isInteracting)
        {
            HandleMovement();
        }
        
        HandleInteraction();
    }

    private void HandleMovement()
    {
        movementInput = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(-movementInput.y, 0f, movementInput.x);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirection, moveDistance, 0, QueryTriggerInteraction.Ignore);

        if(!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDirection.x, 0f, 0f).normalized;
            canMove = moveDirX.x != 0f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirX, moveDistance, 0, QueryTriggerInteraction.Ignore);

            if(canMove)
            {
                moveDirection = moveDirX;
            } else
            {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDirection.z).normalized;
                canMove = moveDirZ.z != 0f && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveDirZ, moveDistance, 0 ,QueryTriggerInteraction.Ignore);
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
        animator.SetBool(IsWalkingHash, isWalking);
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, 0.1f);
    }

    private void HandleInteraction()
    {
        movementInput = GameInput.Instance.GetMovementVector();
        Vector3 moveDirection = new Vector3(-movementInput.y, 0f, movementInput.x);

        if(moveDirection != Vector3.zero)
        {
            lastInteractDir = moveDirection;
        } else
        {
            lastInteractDir = transform.forward;
        }

        Vector3 rayCastOrigin = transform.position + Vector3.up;
        Physics.Raycast(rayCastOrigin, lastInteractDir, out RaycastHit hit, interactionDistance, interactableLayerMask);

        if(hit.collider)
        {
            if(hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if(interactable != selectedInteractable)
                {
                    SetSelectedInteractable(interactable);
                }
            }
            else
            {
                SetSelectedInteractable(null);
            }
        } else
        {
            SetSelectedInteractable(null);
        }
    }

    private void SetSelectedInteractable(IInteractable interactable)
    {
        selectedInteractable = interactable;
    }

    private void OnInteractAction()
    {
        if(!isInteracting && selectedInteractable != null)
        {
            selectedInteractable.Interact();
            isInteracting = true;
            interactedInteractable = selectedInteractable;
        } else if(isInteracting)
        {
            interactedInteractable.ContinueInteract();
        }
    }

    private void OnDialogueEnded(IInteractable interactable)
    {
        isInteracting = false;
        interactedInteractable = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 rayCastOrigin = transform.position + Vector3.up;
        Gizmos.DrawLine(rayCastOrigin, rayCastOrigin + lastInteractDir * 1.5f);
    }
}
}
