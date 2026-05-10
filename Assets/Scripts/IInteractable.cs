using UnityEngine;

public interface IInteractable
{
    bool CanInteract { get; }
    void Interact();
    void ContinueInteract();
    void StopInteract();
    void Highlight();
    void UnHighlight();
}
