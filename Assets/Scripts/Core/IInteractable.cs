namespace DialogSystem.Core
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        void Interact();
        void ContinueInteract();
        void StopInteract();
    }
}
