using System;
using Unity.Cinemachine;
using UnityEngine;
using DialogSystem.Core;

namespace DialogSystem.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private CinemachineCamera playerCamera;
        [SerializeField] private CinemachineCamera dialogCamera;
        private bool hasTarget = false;

        [SerializeField] private DialogueEventChannelSO dialogueEventChannel;

        private void Start() {
            dialogueEventChannel.OnNPCInteracted += OnNPCInteracted;
            dialogueEventChannel.OnStoppedInteraction += OnStoppedInteraction;
        }

        void OnDisable()
        {
            dialogueEventChannel.OnNPCInteracted -= OnNPCInteracted;
            dialogueEventChannel.OnStoppedInteraction += OnStoppedInteraction;
        }

        private void OnStoppedInteraction(IInteractable interactable)
        {
            NPC npc = (NPC)interactable;
            targetGroup.RemoveMember(npc.transform);
            
            playerCamera.gameObject.SetActive(true);
            dialogCamera.gameObject.SetActive(false);
            hasTarget = false;
            
        }

        private void OnNPCInteracted(IInteractable interactable)
        {
            if(hasTarget)
            {
                return;
            }

            NPC npc = (NPC)interactable;
            hasTarget = true;
            targetGroup.AddMember(npc.transform, 1f, 2f);
        
            playerCamera.gameObject.SetActive(false);
            dialogCamera.gameObject.SetActive(true);
        }
    }
}
