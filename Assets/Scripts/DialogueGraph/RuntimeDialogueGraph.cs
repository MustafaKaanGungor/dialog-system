using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDialogueGraph : ScriptableObject
{
    public string EntyNodeID;
    public List<RuntimeDialogueNode> dialogueNodes = new List<RuntimeDialogueNode>();
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