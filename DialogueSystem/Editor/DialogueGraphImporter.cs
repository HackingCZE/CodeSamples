using System;
using System.Collections.Generic;
using System.Linq;
using DH.Core.Managers;
using DialogueSystem;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, DialogueGraph.AssetExtension)]
public class DialogueGraphImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        DialogueGraph graphEditor = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
        DialogueGraphRuntime graphRuntime = ScriptableObject.CreateInstance<DialogueGraphRuntime>();

        var nodeIdMap = new Dictionary<INode, string>();

        foreach(var node in graphEditor.GetNodes())
        {
            nodeIdMap[node] = Guid.NewGuid().ToString();
        }

        var startNode = graphEditor.GetNodes().OfType<DialogueStartNode>().FirstOrDefault();
        if(startNode != null)
        {
            var entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if(entryPort != null) graphRuntime.EntryNodeId = nodeIdMap[entryPort.GetNode()];
        }

        foreach(var node in graphEditor.GetNodes())
        {
            if(node is DialogueEndNode endNode)
            {
                var switchToGraphValue = GetOptionValue<bool>(endNode.GetNodeOptionByName("switchToGraph"));
                if(switchToGraphValue) graphRuntime.switchToGraphs.Add(new()
                {
                    NodeId = nodeIdMap[endNode],
                    toGraph = GetPortValue<DialogueGraphRuntime>(endNode?.GetInputPortByName(DialogueHelper.DIALOGUE_SWITCH_TO_GRAPH))
                });
            }
            
            if(node is not DialogueNode) continue;
            var runtimeNode = new DialogueNodeRuntime() { NodeId = nodeIdMap[node] };
            if(node is DialogueNormalNode nodeNormal)
            {
                ProcessDialogueNode(nodeNormal, runtimeNode, nodeIdMap);
            }
            else if(node is DialogueChoiceNode choiceNode)
            {
                ProcessDialogueNode(choiceNode, runtimeNode, nodeIdMap);
            }


            graphRuntime.Nodes.Add(runtimeNode);
        }

        ctx.AddObjectToAsset("RuntimeData", graphRuntime);
        ctx.SetMainObject(graphRuntime);
    }

    private void ProcessDialogueNode<T>(T node, DialogueNodeRuntime runtimeNode, Dictionary<INode, string> nodeIdMap) where T : DialogueNode
    {
        runtimeNode.IsPlayer = GetOptionValue<bool>(node.GetNodeOptionByName(DialogueHelper.DIALOGUE_IS_PLAYER));
        if(!runtimeNode.IsPlayer) runtimeNode.SpeakerItem = GetPortValue<SpeakerItemSO>(node.GetInputPortByName(DialogueHelper.DIALOGUE_SPEAKER));


        if(typeof(T) == typeof(DialogueNormalNode))
        {
            var locDetailsNode = GetPortValue<DialogueLocalizeDetailsNode>(node.GetInputPortByName(DialogueHelper.DIALOGUE_LOCALIZE_DETAILS));
            ProcessLocalizeDetailsNodes(locDetailsNode, runtimeNode.TextDetails);

            var nextNodePort = node.GetOutputPortByName(DialogueHelper.DIALOGUE_OUT);

            if(nextNodePort?.firstConnectedPort != null)
            {
                runtimeNode.NextNodeId = nodeIdMap[ProcessArray(nextNodePort, out runtimeNode.isEndNode)];
            }
        }
        else if(typeof(T) == typeof(DialogueChoiceNode))
        {
            var choiceOutputPorts = node.GetOutputPorts().Where(p => p.name.StartsWith(DialogueHelper.DIALOGUE_CHOICE_OUT)).ToList();

            foreach(var choiceOutputPort in choiceOutputPorts)
            {
                var index = choiceOutputPort.name.Substring(DialogueHelper.DIALOGUE_CHOICE_OUT.Length);
                var choiceInputPort = node.GetInputPortByName($"{DialogueHelper.DIALOGUE_CHOICE_TEXT_IN}{index}");

                var locDetailsNode = GetPortValue<DialogueLocalizeChoiceNode>(choiceInputPort);
                var textDetails = new DialogueTextDetails();
                ProcessLocalizeDetailsNodes(locDetailsNode, textDetails);

                bool isEndNode = false;
                var choiceNode = choiceOutputPort?.firstConnectedPort != null ? nodeIdMap[ProcessArray(choiceOutputPort, out isEndNode)] : null;
                runtimeNode.AddChoiceData(new DialogueChoiceData()
                {
                    TextDetails = textDetails,
                    ChoiceNodeId = choiceNode,
                    isEndNode = isEndNode
                });
            }
        }
    }

    private INode ProcessArray(IPort port, out bool isEndnode)
    {
        isEndnode = false;
        var outNode = port?.firstConnectedPort.GetNode();
        if(outNode is DialogueArrayNode)
        {
            port = outNode.GetOutputPortByName(DialogueHelper.DIALOGUE_OUT);
            outNode = port?.firstConnectedPort.GetNode();
        }
        
        if(outNode is DialogueEndNode)
        {
            isEndnode = true;
        }
        return outNode;
    }

    private void ProcessLocalizeDetailsNodes<T>(DialogueLanguageNode<T> detailsNode, DialogueTextDetails textDetails) where T : Node
    {
        var lanInputs = detailsNode.GetInputPorts().Where(p => p.name.StartsWith(DialogueHelper.DIALOGUE_LOCALIZE_LANGUAGE_IN)).ToList();
        var msgInputs = detailsNode.GetInputPorts().Where(p => p.name.StartsWith(detailsNode.GetDisplayText)).ToList();
        for(int i = 0; i < lanInputs.Count; i++)
        {
            var lanInput = lanInputs[i];
            var msgInput = msgInputs[i];

            textDetails.AddDialogueLocalizeDetails(
                new DialogueLocalizeDetails()
                {
                    language = GetPortValue<GameManager.Languages>(lanInput),
                    message = GetPortValue<string>(msgInput)
                });
        }
    }

    private T GetOptionValue<T>(INodeOption option)
    {
        if(option == null) return default;

        option.TryGetValue(out T value);
        return value;
    }

    private T GetPortValue<T>(IPort port)
    {
        if(port == null) return default;

        if(port.isConnected)
        {
            if(port.firstConnectedPort.GetNode() is IVariableNode variableNode)
            {
                variableNode.variable.TryGetDefaultValue(out T value);
                return value;
            }
            else if(port.firstConnectedPort.GetNode() != null)
            {
                return (T)port.firstConnectedPort.GetNode();
            }
        }

        port.TryGetValue(out T fallbackValue);
        return fallbackValue;
    }
}