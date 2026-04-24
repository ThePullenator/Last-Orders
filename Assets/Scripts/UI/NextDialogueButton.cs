using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextDialogueButton : MonoBehaviour
{
    Button _button;
    Image _image;
    TMP_Text _text;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _text = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((DialogueManager.Instance.currentCharacterScript.dialogueKeys.Length <= PlayerManager.LastInteractedPerson.dialogueIndex + 1 && DialogueManager.Instance.currentCharacterScript.dialogueOptionScripts.Length !=
            1) || DialogueManager.Instance.currentCharacterScript.GetType() == typeof(RandomizedDialogue))
        {
            HideButton();

            dialogueEvents[] dialogueEventsArray = DialogueManager.Instance.currentCharacterScript.DialogueEventsArray;

            if (DialogueManager.Instance.currentCharacterScript.GetType() != typeof(RandomizedDialogue))
            {
                foreach (dialogueEvents dialogueEvent in dialogueEventsArray)
                {
                    if (!dialogueEventsArray.Contains(dialogueEvent)) DialogueManager.Instance.TriggeredStoryEvents.Add(dialogueEvent);
                    
                    PlayerManager.LastInteractedPerson.setCanBeTalkedTo(false);
                }
            }
            
            if (DialogueManager.Instance.currentCharacterScript.sharedWith.Length != 0)
            {
                foreach (string character in DialogueManager.Instance.currentCharacterScript.sharedWith)
                {
                    DialogueManager.Instance.Characters[character.ToLower()].setCanBeTalkedTo(false);
                }
            }

            PlayerManager.LastInteractedPerson.setCanBeTalkedTo(false);
            PlayerManager.LastInteractedPerson._canBeServed = true;
            
            return;
        }

        if (DialogueManager.Instance.currentCharacterScript.GetType() == typeof(RandomizedDialogue))
        {
            HideButton(); 
            return;
        }
        
        ShowButton();
    }

    void HideButton()
    {
        _image.enabled = false;
        _button.enabled = false;
        _text.enabled = false;
    }

    void ShowButton()
    {
        _image.enabled = true;
        _button.enabled = true;
        _text.enabled = true;
    }
}
