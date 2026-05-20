using System;
using UnityEngine;
using DialogSystem.Core;
using DialogSystem.Dialogue;

namespace DialogSystem.Gameplay
{
    public class NPC : MonoBehaviour, IInteractable
    {
        public CharacterSO characterData;

        public bool CanInteract => true;
        
        private Animator animator;
        [SerializeField] private DialogueEventChannelSO dialogueEventChannel;

        private void Start()
        {
            dialogueEventChannel.OnEmotionTriggered += OnEmotionTriggered;

            animator = GetComponentInChildren<Animator>();
        }

        void OnDisable()
        {
            dialogueEventChannel.OnEmotionTriggered -= OnEmotionTriggered;
        }

        private void OnEmotionTriggered(IInteractable interactable, Emotion emotion)
        {
            if(interactable == this as IInteractable)
            {
                switch(emotion)
                {
                    case Emotion.happy:
                        animator.SetTrigger("happy");
                    break;
                    case Emotion.angry:
                        animator.SetTrigger("angry");
                    break;
                    case Emotion.stretch:
                        animator.SetTrigger("stretch");
                    break;
                    case Emotion.dismiss:
                        animator.SetTrigger("dismiss");
                    break;
                    case Emotion.defeated:
                        animator.SetTrigger("defeated");
                    break;
                    case Emotion.nervous:
                        animator.SetTrigger("nervous");
                    break;
                    
                }
            }
            
        }

        public void Interact()
        {
            dialogueEventChannel.RaiseNPCInteracted(this);
        }

        public void ContinueInteract()
        {
            dialogueEventChannel.RaiseContinueInteraction(this);
        }

        public void StopInteract()
        {
            //? for future use if we want to trigger something when interaction stops
        }

        public void Highlight()
        {
            Outline outline = GetComponentInChildren<Outline>();
            if (outline != null)
                outline.enabled = true;
        }

        public void UnHighlight()
        {
            Outline outline = GetComponentInChildren<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }
}
