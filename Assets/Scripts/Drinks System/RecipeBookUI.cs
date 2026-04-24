using UnityEngine;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RecipeBookUI : MonoBehaviour
{

    private InputAction _recipeBookAction;
    public GameObject bookRoot;
    public Animator bookAnimator;
    public CanvasGroup pageGroup;
    public TMP_Text cocktailName;
    public TMP_Text ingredients;
    public TMP_Text instructions;
    public Image cocktailImage;
    public CocktailRecipe[] recipes;

    public float fadeDuration = 0.25f;
    public float pageTurnDuration = 0.5f;

    int currentIndex = 0;
    bool isOpen;
    bool isBusy;

    void Start()
    {
        _recipeBookAction = PlayerManager.PlayerInput.actions.FindAction("RecipeBook");
        bookRoot.SetActive(false);
        pageGroup.alpha = 0;
    }

   
        void Update()
        {
            if (_recipeBookAction != null && _recipeBookAction.triggered)
            {
                if (!isOpen) OpenBook();
                else CloseBook();
            }
        }
   

    public void OpenBook()
    {
        if (isBusy) return;
        StartCoroutine(OpenRoutine());
    }

    public void CloseBook()
    {
        if (isBusy) return;
        StartCoroutine(CloseRoutine());
    }

    public void NextPage()
    {
        if (isBusy) return;
        if (currentIndex + 1 >= recipes.Length) return;

        currentIndex++;
        StartCoroutine(TurnPage());
     
   
        Debug.Log("next clicked");
    }

    public void PrevPage()
    {
        if (isBusy) return;
        if (currentIndex - 1 < 0) return;

        currentIndex--;
        StartCoroutine(TurnPage());
    }

    IEnumerator OpenRoutine()
    {
        isBusy = true;
        bookRoot.SetActive(true);
        // STOP PLAYER
        PlayerManager.FirstPersonController.enabled = false;
        PlayerManager.PlayerLook.enabled = false;

        // ENABLE MOUSE
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (bookAnimator != null)
            bookAnimator.SetTrigger("TurnPage");
        yield return new WaitForSeconds(pageTurnDuration);
        RefreshPage();
        yield return Fade(0, 1);
        isOpen = true;
        isBusy = false;
    }

    IEnumerator CloseRoutine()
    {
        isBusy = true;
       yield return Fade(1, 0);
        bookRoot.SetActive(false);

        // resume player
        PlayerManager.FirstPersonController.enabled = true;
        PlayerManager.PlayerLook.enabled = true;

        // Lock mouse back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isOpen = false;
        isBusy = false;
    }

    IEnumerator TurnPage()
    {
        isBusy = true;
        yield return Fade(1, 0);
        bookAnimator.SetTrigger("TurnPage");
        yield return new WaitForSeconds(pageTurnDuration);
        RefreshPage();
        yield return Fade(0, 1);
        isBusy = false;
    }

    void RefreshPage()
    {
        var r = recipes[currentIndex];

        cocktailName.text = r.cocktailName;
        ingredients.text = BuildIngredients(r);
        instructions.text = r.instructions;

        if (r.recipeSprite != null)
        {
            cocktailImage.sprite = r.recipeSprite;
            cocktailImage.enabled = true;
        }
        else cocktailImage.enabled = false;
    }

    string BuildIngredients(CocktailRecipe r)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var i in r.requiredIngredients)
            sb.AppendLine("- " + i.ingredient.ingredientName);

        return sb.ToString();
    }

    IEnumerator Fade(float a, float b)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            pageGroup.alpha = Mathf.Lerp(a, b, t / fadeDuration);
            yield return null;
        }

        pageGroup.alpha = b;
    }
}