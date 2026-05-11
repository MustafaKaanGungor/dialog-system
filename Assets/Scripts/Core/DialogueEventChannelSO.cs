using DialogSystem.Dialogue;
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
        public event UnityAction<IInteractable, Emotion> OnEmotionTriggered;

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

        public void RaiseEmotionTriggered(IInteractable interactable, Emotion emotion)
        {
            OnEmotionTriggered?.Invoke(interactable, emotion);
        }
    }
}
