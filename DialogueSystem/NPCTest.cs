using System;
using Interfaces;
using UnityEngine;


public class NPCTest : DialogueSystem.DialogueBase, IInteractable
{
   public bool CanInteract { get; set; } = true;
   public void Interact()
   {
      DialogueSystem.DialogueManager.Instance.Configure(this);
   }
}
