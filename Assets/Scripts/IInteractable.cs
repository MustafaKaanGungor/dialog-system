using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    void Interact();
    void StopInteract();
    void Highlight();
    void UnHighlight();
}
