using System;
using UnityEngine;

public enum StoryEvents
{
    None
}

[CreateAssetMenu(fileName = "DialogueConditional", menuName = "Scriptable Objects/DialogueConditional")]
public class DialogueConditional : ScriptableObject
{
    public DialogueConditionalItem[] possibleDialogues;

    public CharacterScript GetCharacterScript()
    {
       TimeSpan currentTime = TimeManager.Instance.currentTime;

       for (int i = 0; i < possibleDialogues.Length; i++)
       {
           DialogueConditionalItem conditionalItem = possibleDialogues[i];

           //if (CheckIfValidConditional(conditionalItem) && CheckTime(conditionalItem, currentTime))
           //{
           //    return conditionalItem.CharacterScript;
           //}
       }
       
       Debug.LogError("No valid dialogues found!");
       
       //Return last item if error
       return possibleDialogues[^1].CharacterScript;
    }

    TimeSpan GetStartTimespan(DialogueConditionalItem conditionalItem)
    {
        return new TimeSpan(conditionalItem.startHour, conditionalItem.startMinute, 0);
    }
    
    TimeSpan GetEndTimespan(DialogueConditionalItem conditionalItem)
    {
        return new TimeSpan(conditionalItem.endHour, conditionalItem.endMinute, 0);
    }

    bool CheckIfValidConditional(DialogueConditionalItem conditionalItem)
    {
        //return DialogueManager.Instance.TriggeredStoryEvents.Contains(conditionalItem.triggeringEvent);

        return true;
    }

    bool CheckTime(DialogueConditionalItem conditionalItem, TimeSpan currentTime)
    {
        TimeSpan startSpan = GetStartTimespan(conditionalItem);
        TimeSpan endSpan = GetEndTimespan(conditionalItem);
        
        if (startSpan.Hours == endSpan.Hours)
        {
            return currentTime.Minutes < endSpan.Minutes && currentTime.Minutes > startSpan.Minutes;
        }

        return currentTime.Hours < endSpan.Hours && currentTime.Hours > startSpan.Hours;
    }
}

[System.Serializable]
public class DialogueConditionalItem
{
    public StoryEvents triggeringEvent;
    [Range(0,59)] public int startMinute;
    [Range(0,24)] public int startHour;
    [Range(0,59)] public int endMinute;
    [Range(0,24)] public int endHour;
    public bool barConvo;
    public CharacterScript CharacterScript;
}