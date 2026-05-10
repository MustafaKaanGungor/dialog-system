using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueEventChannelSO", menuName = "Dialogue System/Event Channel", order = 1)]
public class DialogueEventChannelSO : ScriptableObject
{
    public UnityAction<NPC> OnNPCInteracted;
    public UnityAction<NPC> OnContinueInteraction;
    public UnityAction<NPC> OnStoppedInteraction;

    public void RaiseNPCInteracted(NPC npc)
    {
        OnNPCInteracted?.Invoke(npc);
    }

    public void RaiseContinueInteraction(NPC npc)
    {
        OnContinueInteraction?.Invoke(npc);
    }

    public void RaiseStoppedInteraction(NPC npc)
    {
        OnStoppedInteraction?.Invoke(npc);
    }
}
