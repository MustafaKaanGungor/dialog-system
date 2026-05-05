using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera dialogCamera;
    private bool hasTarget = false;

    private void Start() {
        Player.Instance.NPCInteracted += OnNPCInteracted;
        Player.Instance.StoppedInteraction += OnStoppedInteraction;
    }

    private void OnStoppedInteraction(NPC npc)
    {
        targetGroup.RemoveMember(npc.transform);
        
        playerCamera.Priority.Value = 1;
        dialogCamera.Priority.Value = 0;
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
    
        playerCamera.Priority.Value = 0;
        dialogCamera.Priority.Value = 1;
    }
}
