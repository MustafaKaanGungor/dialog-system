using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDialogueGraph : ScriptableObject
{
    public string EntryNodeID;
    public List<RuntimeDialogueNode> dialogueNodes = new List<RuntimeDialogueNode>();

    private void OnValidate()
    {
        if(EntryNodeID == null || EntryNodeID == "")
        {
            Debug.LogError("Dialogue Graph has no entry node", this);
        }

        foreach (RuntimeDialogueNode node in dialogueNodes)
        {
            if(string.IsNullOrEmpty(node.NextNodeID) && node.choices.Count == 0)
            {
                Debug.LogError("Dialogue Graph has a node with no next node and no choices", this);
            }

            if(node.choices.Count > 0)
            {
                foreach (ChoiceData choice in node.choices)
                {
                    if(string.IsNullOrEmpty(choice.DestinationID))
                    {
                        Debug.LogError("Dialogue Graph has a choice with no destination node", this);
                    }
                }
            }
        }
    }
}

[Serializable]
public class RuntimeDialogueNode
{
    public string NodeID;
    public string DialogueText;
    public List<ChoiceData> choices = new List<ChoiceData>();
    public string NextNodeID;
}

[Serializable] 
public class ChoiceData
{
    public string ChoiceText;
    public string DestinationID;
}