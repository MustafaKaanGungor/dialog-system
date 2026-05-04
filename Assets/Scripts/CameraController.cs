using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera dialogCamera;

    private void Start() {
        Player.Instance.NPCInteracted += OnNPCInteracted;
        Player.Instance.StoppedInteraction += OnStoppedInteraction;
    }

    private void OnStoppedInteraction(NPC npc)
    {
        targetGroup.RemoveMember(npc.transform);
        playerCamera.Priority = 1;
        dialogCamera.Priority = 0;
    }

    private void OnNPCInteracted(NPC npc)
    {
        targetGroup.AddMember(npc.transform, 1f, 2f);
        playerCamera.Priority = 0;
        dialogCamera.Priority = 1;
    }
}
