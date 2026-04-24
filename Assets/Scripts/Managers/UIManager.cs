using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static event Action<RaycastHit> OnObjectHovered;
    public TMP_Text timeText;
    public GameObject customerUI;
    public GameObject notepadUI;
    public ShowInput inputDisplay;
    public TextMeshProUGUI itemTextUI;
    
    [SerializeField] Button serveButton;
    [SerializeField] CanvasGroup promptCanvasGroup;
    [SerializeField] Color crosshairColour;
    [SerializeField] Color shakerHoverColor;
    [SerializeField] Image crosshair;
    [SerializeField] Transform barExit;
    
    float _reach;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Instance) Debug.LogError("There is multiple UIMangers, there should only be 1!");
        _reach = PlayerManager.FirstPersonController.interactRange;
        
        Cursor.lockState = CursorLockMode.Locked;
        Instance = this;
    }

    public void CloseUI(GameObject ui)
    {
        PlayerManager.LastInteractedPerson.cam.SetActive(false);
        PlayerManager.LastInteractedPerson = null;
        PlayerManager.FirstPersonController.enabled = true;
        PlayerManager.PlayerLook.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        ui.SetActive(false);
    }

    public void ToggleNotepadUI()
    {
        if (notepadUI.activeSelf)
        {
            notepadUI.SetActive(false);
            PlayerManager.PlayerLook.enabled = true;
            if (PlayerManager.LastInteractedPerson == null) Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            notepadUI.SetActive(true);
            PlayerManager.PlayerLook.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ServeDrink()
    {
        PlayerManager.currentDrink.OnDrink(PlayerManager.LastInteractedPerson);
        PlayerManager.currentDrink = null;
        
        //This is temp stuff, fix this when u figure out how to twin characters
        if (PlayerManager.LastInteractedPerson.characterName == "Carmen")
        {
            Destroy(DialogueManager.Instance.Characters["sapphire"].gameObject);
            DialogueManager.Instance.Characters.Remove("sapphire");
        }
        
        if(!PlayerManager.LastInteractedPerson.unique) PlayerManager.LastInteractedPerson.SwitchStates(new MoveToState(barExit.position, true));
        
        CloseUI(customerUI);
    }

    void Update()
    {
        CheckForObject(Camera.main.transform);
        
        //This is for the server button,
        if (PlayerManager.currentDrink != null) serveButton.interactable = true;
        else serveButton.interactable = false;
    }

    void CheckForObject(Transform origin)
    {
        Ray ray = new Ray(origin.position, origin.forward);
        RaycastHit hit;
        GameObject hitObject;
        
        // Ignore NPCs when holding item
        int pickupLayer = LayerMask.NameToLayer("Pickup");
        int shakerLayer = LayerMask.NameToLayer("Shaker");
        int ignoreMask = PlayerManager.FirstPersonController.interactMask ;
        
        Debug.DrawLine(ray.origin, origin.forward * _reach, Color.green);
        crosshair.color = crosshairColour;
        
        if (Physics.Raycast(ray, out hit, _reach, PlayerManager.FirstPersonController.interactMask))
        {
            hitObject = hit.collider.gameObject;
            OnObjectHovered?.Invoke(hit);
            
            if (hitObject.CompareTag("Shaker"))
            {
                crosshair.color = shakerHoverColor;
            }
            
            // Make sure the UI is visible
            if (promptCanvasGroup != null)
                promptCanvasGroup.alpha = 1;

            if (itemTextUI != null)
                if (itemTextUI != null)
                    switch (hitObject.layer)
                    {
                        default:
                            Debug.LogWarning("Need a dedicated layer for this, remaining blank until then");
                            itemTextUI.text = "";
                            break;
                        
                        case 8: // NPC layer
                            Person npcModule = hitObject.GetComponent<Person>();
                            itemTextUI.text = $"Talk to {npcModule.characterName}";
                            break;
                        
                        case 9: // Pickup layer (could be more pickupable items in the future, maybe use another switch statement instead?)
                            if (hitObject.CompareTag("Bottle"))
                            {
                                Bottle bottleObjModule = hitObject.GetComponent<Bottle>();
                                itemTextUI.text = $"Pick up {bottleObjModule.Ingredient.ingredientName}";
                            }
                            else
                            {
                                itemTextUI.text = $"Pick up {hitObject.name}";
                            }
                            break;
                        
                        case 11: // Shaker layer
                            itemTextUI.text = $"Interact with {hitObject.name}";
                            break;
                        
                        case 12: // Interactable layer (jukebox, etc)
                            itemTextUI.text = $"Interact with {hitObject.name}";
                            break;
                    }

            // itemTextUI.text = $"{hitObject.name}";

            return;
        }
        
        if (promptCanvasGroup != null) promptCanvasGroup.alpha = 0;
    }
    public void EnterMinigameMode()
    {
        PlayerManager.FirstPersonController.enabled = false;
        PlayerManager.PlayerLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitMinigameMode()
    {
        PlayerManager.FirstPersonController.enabled = true;
        PlayerManager.PlayerLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
