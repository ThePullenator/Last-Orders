using UnityEngine;

public enum dialogueEvents
{
    
}

[CreateAssetMenu(fileName = "CharacterScript", menuName = "Scriptable Objects/CharacterScript")]
public class CharacterScript : ScriptableObject
{
    public string[] dialogueKeys;
    public CharacterScript[] dialogueOptionScripts;
    public dialogueEvents[] DialogueEventsArray;
    public string[] sharedWith;

    public virtual string GetKey(int index)
    {
        return dialogueKeys[index];
    }
}
