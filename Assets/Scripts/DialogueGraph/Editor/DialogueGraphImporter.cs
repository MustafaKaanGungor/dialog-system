using UnityEngine;
using UnityEditor.AssetImporters;
using Unity.GraphToolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

[ScriptedImporter(1, DialogueGraph.AssetExtension)]
public class DialogueGraphImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        DialogueGraph editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
        RuntimeDialogueGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();
        var NodeIDMap = new Dictionary<INode, string>();

        foreach (var node in editorGraph.GetNodes())
        {
            NodeIDMap[node] = Guid.NewGuid().ToString();
        }

        var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();

        if(startNode != null)
        {
            var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if(entryPort != null)
            {
                runtimeGraph.EntryNodeID = NodeIDMap[entryPort.GetNode()];
            }
        }

        foreach(var iNode in editorGraph.GetNodes())
        {
            if(iNode is StartNode || iNode is EndNode)
            {
                continue;
            }

            var runtimeNode = new RuntimeDialogueNode
            {
                NodeID = NodeIDMap[iNode]
            };

            if(iNode is DialogueNode dialogueNode)
            {
                ProcessDialogueNode(dialogueNode, runtimeNode, NodeIDMap);
            } else if(iNode is ChoiceNode choiceNode)
            {
                ProcessChoiceNode(choiceNode, runtimeNode, NodeIDMap);
            }

            runtimeGraph.dialogueNodes.Add(runtimeNode);
        }

        ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
        ctx.SetMainObject(runtimeGraph);
    }

    private void ProcessDialogueNode(DialogueNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("DialogueLine"));

        var nextNodePort = node.GetOutputPortByName("out")?.firstConnectedPort;

        if(nextNodePort != null)
        {
            runtimeNode.NextNodeID = nodeIDMap[nextNodePort.GetNode()];
        }

    }

    private void ProcessChoiceNode(ChoiceNode node, RuntimeDialogueNode runtimeNode, Dictionary<INode, string> nodeIDMap)
    {
        runtimeNode.DialogueText = GetPortValue<string>(node.GetInputPortByName("DialogueLine"));

        var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith("Choice "));

        foreach (var port in choiceOutputPorts)
        {
            var index = port.name.Substring("Choice ".Length);
            var textPort = node.GetInputPortByName($"Choice Text {index}");

            var ChoiceData = new ChoiceData()
            {
                ChoiceText = GetPortValue<string>(textPort),
                DestinationID = port.firstConnectedPort != null ? nodeIDMap[port.firstConnectedPort.GetNode()] : null
            };

            runtimeNode.choices.Add(ChoiceData);
        }
    }

    private T GetPortValue<T>(IPort port)
    {
        if(port == null)
        {
            return default;
        }

        if(port.isConnected)
        {
            if(port.firstConnectedPort.GetNode() is IVariableNode variableNode)
            {
                variableNode.variable.TryGetDefaultValue(out T value);
                return value;
            }
        }

        port.TryGetValue(out T fallback);
        return fallback;
    }
}
