using System;
using System.Collections.Generic;
using DH.Core.Helpers.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public DialogueBase currentDialogue;
        [SerializeField] private RectTransform root;
        [SerializeField] private TMPro.TMP_Text messageText;
        [SerializeField] private Image imageNPCProfile;
        [SerializeField] private Image imagePlayerProfile;
        [SerializeField] private Image colorIndicator;
        [SerializeField] private Button nextButton;

        [SerializeField] private RectTransform nextBtnRoot;
        [SerializeField] private RectTransform otherBtnRoot;
        
        public bool IsActive => root.gameObject.activeInHierarchy;
        
        [Expandable]
        public DialogueGraphRuntime graphRuntime;
        private Dictionary<string, DialogueNodeRuntime> nodes = new Dictionary<string, DialogueNodeRuntime>();
        private DialogueNodeRuntime currentDialogueNode;

        private void Start()
        {
            foreach(var node in graphRuntime.Nodes)
            {
                nodes[node.NodeId] = node;
            }
        }

        public void Configure(DialogueBase dialogue)
        {
            if(dialogue != null && IsActive) return;
            currentDialogue = dialogue;
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                this.ShowDialogueItem(currentDialogue.NextDialogue());
            });
            this.ShowDialogueItem(currentDialogue.NextDialogue());
        }

        private void ShowDialogueItem(DialogueItemSO dialogueItemSo)
        {
            if(dialogueItemSo == null)
            {
                ActiveRoot(false);
                currentDialogue = null;
                return;
            }
            // ActiveRoot(true);
            // messageText.text = dialogueItemSo.Message;
            // imageNPCProfile.sprite = dialogueItemSo.speakerItem.GetMoodItem(dialogueItemSo.mood).profileIcon;
            // colorIndicator.color = dialogueItemSo.color;
            // HighlightProfile(dialogueItemSo.isPlayer, imagePlayerProfile);
            // HighlightProfile(!dialogueItemSo.isPlayer, imageNPCProfile);
        }

        public void ActiveRoot(bool value)
        {
            root.gameObject.SetActive(value);
        }

        public void HighlightProfile(bool value, Image image)
        {
            image.gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = value;
        }
        
    }

    public static class DialogueHelper
    {
        public const string DIALOGUE_LOCALIZE_DETAILS = "LocalizeDetails";
        public const string DIALOGUE_CHOICE_TEXT_IN = "ChoiceText ";
        public const string DIALOGUE_CHOICE_OUT = "Choice ";
        
        public const string DIALOGUE_LOCALIZE_LANGUAGE_IN = "Language ";
        public const string DIALOGUE_LOCALIZE_CHOICE_NODE_IN = "Text ";
        public const string DIALOGUE_LOCALIZE_DETAILS_NODE_IN = "Message ";
        
        public const string DIALOGUE_SPEAKER = "Speaker";
        public const string DIALOGUE_MOOD = "Mood";
        public const string DIALOGUE_COLOR = "Color";
        public const string DIALOGUE_IS_PLAYER = "isPlayer";
        
        public const string DIALOGUE_OUT = "out";
        public const string DIALOGUE_IN= "in";
        
        public const string DIALOGUE_SWITCH_TO_GRAPH = "Switch to Graph";
        
    }
}