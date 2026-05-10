using System;
using Unity.Cinemachine;
using UnityEngine;

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

    private void OnStoppedInteraction(NPC npc)
    {
        targetGroup.RemoveMember(npc.transform);
        
        playerCamera.gameObject.SetActive(true);
        dialogCamera.gameObject.SetActive(false);
        hasTarget = false;
        
    }

    private void OnNPCInteracted(NPC npc)
    {
        if(hasTarget)
        {
            return;
        }

        hasTarget = true;
        targetGroup.AddMember(npc.transform, 1f, 2f);
    
        playerCamera.gameObject.SetActive(false);
        dialogCamera.gameObject.SetActive(true);
    }
}
