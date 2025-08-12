using System;
using System.Collections.Generic;
using DH.Core.Managers;
using DialogueSystem;
using NaughtyAttributes;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DialogueStartNode : Node
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddOutputPort<BaseDialogueNode>(DialogueHelper.DIALOGUE_OUT).Build();
    }
}

[Serializable]
public class DialogueEndNode : Node
{
    private const string switchToGraph = "switchToGraph";
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<BaseDialogueNode>(DialogueHelper.DIALOGUE_IN).Build();
        
        var optionValue = GetNodeOptionByName(switchToGraph);
        optionValue.TryGetValue(out bool switchToGraphValue);
        if(switchToGraphValue)
        {
            context.AddInputPort<DialogueGraphRuntime>(DialogueHelper.DIALOGUE_SWITCH_TO_GRAPH).Build();
        }
    }

    protected override void OnDefineOptions(INodeOptionDefinition context)
    {
        base.OnDefineOptions(context);
        context.AddNodeOption<bool>(switchToGraph);
    }
}


[Serializable]
public abstract class BaseDialogueNode : Node
{
}

[Serializable]
public class DialogueArrayNode : BaseDialogueNode
{
    private const string count = "count";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        var optionValue = GetNodeOptionByName(count);
        optionValue.TryGetValue(out int countValue);
        var maxCount = 8;
        if(countValue < 2) countValue = 2;
        for(int i = 0; i < Mathf.Min(countValue, maxCount); i++)
        {
            context.AddInputPort<BaseDialogueNode>($"in {i}").Build();
        }

        context.AddOutputPort<DialogueArrayNode>(DialogueHelper.DIALOGUE_OUT).Build();
    }

    protected override void OnDefineOptions(INodeOptionDefinition context)
    {
        context.AddNodeOption<int>(count, defaultValue: 1, attributes: new Attribute[] { new MinValueAttribute(1), new MaxValueAttribute(15) });
    }
}

[Serializable]
public abstract class DialogueNode : BaseDialogueNode
{
    private const string isPlayer = DialogueHelper.DIALOGUE_IS_PLAYER;

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddInputPort<BaseDialogueNode>(DialogueHelper.DIALOGUE_IN).Build();

        context.AddInputPort<SpeakerItemSO.Moods>(DialogueHelper.DIALOGUE_MOOD).Build();
        context.AddInputPort<Color>(DialogueHelper.DIALOGUE_COLOR).Build();

        var optionValue = GetNodeOptionByName(isPlayer);
        optionValue.TryGetValue(out bool isPlayerValue);
        if(!isPlayerValue)
        {
            context.AddInputPort<SpeakerItemSO>(DialogueHelper.DIALOGUE_SPEAKER).Build();
        }
    }

    protected override void OnDefineOptions(INodeOptionDefinition context)
    {
        context.AddNodeOption<bool>(isPlayer);
    }
}


[Serializable]
public class DialogueNormalNode : DialogueNode
{
    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        base.OnDefinePorts(context);
        context.AddOutputPort<BaseDialogueNode>(DialogueHelper.DIALOGUE_OUT).Build();

        context.AddInputPort<DialogueLocalizeDetailsNode>(DialogueHelper.DIALOGUE_LOCALIZE_DETAILS).Build();
    }
}

[Serializable]
public class DialogueChoiceNode : DialogueNode
{
    private const string countChoice = "countChoice";

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        base.OnDefinePorts(context);

        var optionValue = GetNodeOptionByName(countChoice);
        optionValue.TryGetValue(out int countChoiceValue);
        var maxCount = 7;
        if(countChoiceValue < 2) countChoiceValue = 2;
        for(int i = 0; i < Mathf.Min(countChoiceValue, maxCount); i++)
        {
            context.AddInputPort<DialogueLocalizeChoiceNode>($"{DialogueHelper.DIALOGUE_CHOICE_TEXT_IN}{i}").Build();
            context.AddOutputPort<BaseDialogueNode>($"{DialogueHelper.DIALOGUE_CHOICE_OUT}{i}").Build();
        }
    }

    protected override void OnDefineOptions(INodeOptionDefinition context)
    {
        base.OnDefineOptions(context);
        context.AddNodeOption<int>(countChoice, defaultValue: 2, attributes: new Attribute[] { new DelayedAttribute(), new MinValueAttribute(2), new MaxValueAttribute(7) });
    }
}

[Serializable]
public abstract class DialogueLanguageNode<T> : Node where T : Node
{
    public const string countOfLocalized = "count";
    public abstract string GetDisplayText { get; }

    protected override void OnDefinePorts(IPortDefinitionContext context)
    {
        context.AddOutputPort<T>(DialogueHelper.DIALOGUE_OUT).Build();

        var optionValue = GetNodeOptionByName(countOfLocalized);
        optionValue.TryGetValue(out int countOfLocalizedValue);
        var maxCount = Enum.GetNames(typeof(GameManager.Languages)).Length;
        if(countOfLocalizedValue < 1) countOfLocalizedValue = 1;
        for(int i = 0; i < Mathf.Min(countOfLocalizedValue, maxCount); i++)
        {
            context.AddInputPort<GameManager.Languages>($"{DialogueHelper.DIALOGUE_LOCALIZE_LANGUAGE_IN}{i}").Build();
            context.AddInputPort<string>($"{GetDisplayText}{i}").Build();
        }
    }

    protected override void OnDefineOptions(INodeOptionDefinition context)
    {
        context.AddNodeOption<int>(countOfLocalized, defaultValue: 1, attributes: new Attribute[] { new MinValueAttribute(1), new MaxValueAttribute(Enum.GetNames(typeof(GameManager.Languages)).Length) });
    }
}

[Serializable]
public class DialogueLocalizeDetailsNode : DialogueLanguageNode<DialogueLocalizeDetailsNode>
{
    public override string GetDisplayText => DialogueHelper.DIALOGUE_LOCALIZE_DETAILS_NODE_IN;
}

[Serializable]
public class DialogueLocalizeChoiceNode : DialogueLanguageNode<DialogueLocalizeChoiceNode>
{
    public override string GetDisplayText => DialogueHelper.DIALOGUE_LOCALIZE_CHOICE_NODE_IN;
}