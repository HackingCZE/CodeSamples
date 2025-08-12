using System;
using System.Collections.Generic;
using DH.Core.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace DialogueSystem
{
    
    public class DialogueGraphRuntime : ScriptableObject
    {
        public string EntryNodeId;
        public List<DialogueEndNode> switchToGraphs = new List<DialogueEndNode>();
        public List<DialogueNodeRuntime> Nodes = new List<DialogueNodeRuntime>();
    }
    
    [Serializable]
    public class DialogueNodeRuntime
    {
        [ReadOnly]
        public string NodeId;
        
        [Tooltip("Talking player?")] public bool IsPlayer = false;
        public SpeakerItemSO.Moods Mood;
        
        [HideIf("IsPlayer"), Expandable] public SpeakerItemSO SpeakerItem;
       
        [ReadOnly]
        public string NextNodeId;
        public bool isEndNode = false;

        public DialogueTextDetails TextDetails = new();
        public List<DialogueChoiceData> Choices = new();

        public void AddChoiceData(DialogueChoiceData choiceData)
        {
            if (Choices == null) 
                Choices = new();
            
            Choices.Add(choiceData);
        }
        
    }

    [Serializable]
    public class DialogueEndNode
    {
        public string NodeId;
        public DialogueGraphRuntime toGraph;
    }

    [Serializable]
    public class DialogueTextDetails
    {
        public List<DialogueLocalizeDetails> LocalizeDetails = new();

        public string DialogueText => LocalizeDetails[0].message; // TODO: make by selected language
        
        public void AddDialogueLocalizeDetails(DialogueLocalizeDetails details)
        {
            if (LocalizeDetails == null) 
                LocalizeDetails = new List<DialogueLocalizeDetails>();
            
            LocalizeDetails.Add(details);
        }
    }
    
    [Serializable]
    public class DialogueChoiceData
    {
        public DialogueTextDetails TextDetails;
        public string ChoiceNodeId;
        public bool isEndNode = false;
    }
    [Serializable]
    public class DialogueLocalizeDetails
    {
        public GameManager.Languages language;
        public string message;
    }
        
    [Serializable]
    public class DialogueLocalizeChoice
    {
        public GameManager.Languages language;
        public string text;
    }
}