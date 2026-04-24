using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using Newtonsoft.Json;
using UnityEngine;
using FMOD;
using FMODUnity;
using Debug = UnityEngine.Debug;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Dictionary<string, string> DialogueDict;
    public Dictionary<string, Person> Characters = new Dictionary<string, Person>();
    public CharacterScript currentCharacterScript;
    public HashSet<dialogueEvents> TriggeredStoryEvents;
    public GameObject choiceButtonPrefab;
    public StudioEventEmitter speakTick;
    
    string _currentKey = "generic_skibidi_closed_0";
    TextAsset jsonFile;
    readonly string _path = "Resources/dialogue.json";
    TMP_Text _dialogueText;
    TMP_Text _characterSpeaking;
    GameObject _lastCam;
    
    
    void Awake()
    {
        if (Instance) Debug.LogError("There are multiple dialogue managers! there should only be 1!");
        Instance = this;
        
        jsonFile = Resources.Load<TextAsset>("dialogue");
        
        _dialogueText = transform.GetChild(0).GetComponent<TMP_Text>();
        _characterSpeaking = transform.GetChild(2).GetChild(0).GetComponent<TMP_Text>();
        
        GetDialogueJson();
    }

    void GetDialogueJson()
    {
        string path = Path.Combine(Application.dataPath, _path);
        string existingJson = jsonFile.text;

        DialogueDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(existingJson);
    }

    string GetNextText(string currentKey)
    {
        int index = GetEndOfKey(currentKey) + 1;
        string tempKey = RemoveEndOfKey(currentKey);

        return tempKey + $"_{index}";
    }

    public void NextDialogue()
    {
        string nextText = GetNextText(_currentKey);
        
        Debug.Log(nextText);
        
        if (DialogueDict.ContainsKey(nextText))
        {
            _dialogueText.SetText(DialogueDict[nextText]);
            _currentKey = nextText;
          //  speakTick.Play();
        }
        else Debug.Log("Final text!");
    }

    public void NextDialogueInScript()
    {
        if (currentCharacterScript == null) return;
        int index = PlayerManager.LastInteractedPerson.dialogueIndex;
        
        
        //This runs if theres only one choice
        if (index + 1 == currentCharacterScript.dialogueKeys.Length)
        {
            if (currentCharacterScript.dialogueOptionScripts.Length != 1) return;
            
            currentCharacterScript = currentCharacterScript.dialogueOptionScripts[0];
            PlayerManager.LastInteractedPerson.dialogue = currentCharacterScript;
            
            ShowText();
            return;
        }
        
        index++;
        
        PlayerManager.LastInteractedPerson.dialogueIndex = index;

        string key = currentCharacterScript.GetKey(index);
        
        SwapCams(key);

        if (GetDialogueType(key) == "choice")
        {
            GenerateChoices(DialogueDict[key]);
            return;
        }
        
        if (GetCharacterFromKey(key) != "Generic") _characterSpeaking.SetText(GetCharacterFromKey(key));
        else _characterSpeaking.SetText(PlayerManager.LastInteractedPerson.characterName);
        
        _dialogueText.SetText(FormatText(DialogueDict[key]));
    }

    void SwapCams(string key)
    {
        if(_lastCam != null) _lastCam?.SetActive(false);
        Person characterSpeaking = Characters[GetCharacterFromKey(key).ToLower()];
        characterSpeaking?.SwapToCamera();
        _lastCam = characterSpeaking?.cam;
    }

    void GenerateChoices(string choice)
    {
        Transform choiceBoxLayout = transform.GetChild(3);
        
        choiceBoxLayout.gameObject.SetActive(true);
        
        string[] choices = choice.Split("|");

        for (int i = 0; i < choices.Length; i++)
        {
           GameObject choiceBox = Instantiate(choiceButtonPrefab, choiceBoxLayout);
           TMP_Text choiceBoxText = choiceBox.transform.GetChild(0).GetComponent<TMP_Text>();

           choiceBoxText.SetText(FormatChoiceText(choices[i]));
           choiceBox.GetComponent<ChoiceBox>().choiceIndex = i;
        }
    }

    public void SetChoice(int index)
    {
        currentCharacterScript = currentCharacterScript.dialogueOptionScripts[index];
        PlayerManager.LastInteractedPerson.dialogue = currentCharacterScript;
        PlayerManager.LastInteractedPerson.dialogueIndex = 0;
        
        SwapCams(currentCharacterScript.GetKey(0));
        
        ShowText();
    }

    public string FormatChoiceText(string text)
    {
        return text.Split("<")[0];
    }
    
    
    //I'm not even going to pretend I fully understand how regex works.
    public string FormatText(string text)
    {
        return Regex.Replace(text, @"\[(.*?)\]", $"[{PlayerManager.LastInteractedPerson.Drink.Name}]");
    }

    public void ShowText()
    {
        string key = currentCharacterScript.GetKey(PlayerManager.LastInteractedPerson.dialogueIndex);
        if(!DialogueDict.ContainsKey(key)) return;

        if (GetDialogueType(key) == "choice") key = currentCharacterScript.GetKey(PlayerManager.LastInteractedPerson.dialogueIndex-1);
        
        Debug.Log(GetCharacterFromKey(key));
        
        if (GetCharacterFromKey(key) != "Generic") _characterSpeaking.SetText(GetCharacterFromKey(key));
        else _characterSpeaking.SetText(PlayerManager.LastInteractedPerson.characterName);
        
        _dialogueText.SetText(FormatText(DialogueDict[key]));
    }
    
    public static int GetEndOfKey(string key)
    {
        int lastUnderscore = key.LastIndexOf("_");
        int.TryParse(key.Substring(lastUnderscore + 1), out int result);
        return result;
    }

    public static string GetCharacterFromKey(string key)
    {
        int firstUnderscore = key.IndexOf("_");
        string characterName = key.Substring(0, firstUnderscore);
        string capitalizedName = char.ToUpper(characterName[0]) + characterName.Substring(1);
        return capitalizedName;
    }

    public static string RemoveEndOfKey(string key)
    {
        int lastUnderscore = key.LastIndexOf("_");
        return key.Substring(0, lastUnderscore);
    }

    public void ForceSetText(string text)
    {
        _dialogueText.SetText(FormatText(text));
    }

    public void ForceSetCharacter(string character)
    {
        _characterSpeaking.SetText(character);
    }

    public static string GetDialogueType(string key)
    {
        string tempKey = RemoveEndOfKey(key);
        int lastUnderscore = tempKey.LastIndexOf("_");
        return tempKey.Substring(lastUnderscore + 1);
    }
}
