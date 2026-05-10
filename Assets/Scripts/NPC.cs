using System;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public CharacterSO characterData;

    public bool CanInteract => true;
    
    [SerializeField] private DialogueEventChannelSO dialogueEventChannel;

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
