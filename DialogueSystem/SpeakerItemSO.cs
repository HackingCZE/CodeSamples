using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DH.Core.Managers;
using DialogueSystem;
namespace DialogueSystem
{
[CreateAssetMenu(menuName = "Tonikum/Dialogue System/NPC Item")]
public class SpeakerItemSO : ScriptableObject
{
    public List<MoodItem> moodItems = new List<MoodItem>();
    
    public MoodItem GetMoodItem(Moods mood)
    {
        MoodItem details = moodItems.FirstOrDefault(d => d.mood == mood);

        if(details != null)
        {
            return details;
        }
        else
        {
            MoodItem defaultDetails = moodItems.FirstOrDefault(d => d.mood == Moods.Normal);
            if(defaultDetails != null)
            {
                Debug.LogWarning($"Zpráva pro jazyk {mood} nebyla nalezena pro {this.name}. Používám anglickou zprávu.");
                return defaultDetails;
            }
            else
            {
                Debug.LogError($"Zpráva pro jazyk {mood} ani výchozí anglická zpráva nebyla nalezena pro {this.name}!");
                return null;
            }
        }
    }
    
    [Serializable]
    public class MoodItem
    {
        public Moods mood;
        public Sprite profileIcon;
    }
    public enum Moods
    {
        Normal,
        Angry,
        Sad,
        Happy,
    }
}
}
