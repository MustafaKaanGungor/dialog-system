using UnityEngine;
using UnityEngine.Events;

namespace DialogSystem.Core
{
    [CreateAssetMenu(fileName = "DialogueEventChannelSO", menuName = "Dialogue System/Event Channel", order = 1)]
    public class DialogueEventChannelSO : ScriptableObject
    {
        public event UnityAction<IInteractable> OnNPCInteracted;
        public event UnityAction<IInteractable> OnContinueInteraction;
        public event UnityAction<IInteractable> OnStoppedInteraction;

        public void RaiseNPCInteracted(IInteractable interactable)
        {
            OnNPCInteracted?.Invoke(interactable);
        }

        public void RaiseContinueInteraction(IInteractable interactable)
        {
            OnContinueInteraction?.Invoke(interactable);
        }

        public void RaiseStoppedInteraction(IInteractable interactable)
        {
            OnStoppedInteraction?.Invoke(interactable);
        }
    }
}
