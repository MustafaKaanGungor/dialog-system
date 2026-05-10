using UnityEngine;
using UnityEngine.Events;

namespace DialogSystem.Core
{
    [CreateAssetMenu(fileName = "DialogueEventChannelSO", menuName = "Dialogue System/Event Channel", order = 1)]
    public class DialogueEventChannelSO : ScriptableObject
    {
        public UnityAction<IInteractable> OnNPCInteracted;
        public UnityAction<IInteractable> OnContinueInteraction;
        public UnityAction<IInteractable> OnStoppedInteraction;

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
