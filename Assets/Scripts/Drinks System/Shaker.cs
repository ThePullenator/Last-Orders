using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System.Collections;

public class Shaker : MonoBehaviour, IInteractable
{
    public static Shaker Current;

    public Transform spawnPoint;
    public GameObject failedDrinkPrefab;
    public GameObject defaultDrink;
    public Vector3 drinkSpawnOfsset;

    List<IngredientData> _addedIngredients = new List<IngredientData>();
    CocktailRecipe pendingRecipe;
    bool waiting;
    [SerializeField] float textDisplayTime = 5f;
    Coroutine textRoutine;
    TMP_Text text;
    ShakerText _shakerText;


    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        _shakerText = GetComponentInChildren<ShakerText>();
    }

    public void AddIngredient(IngredientData ingredient)
    {
        if (!_addedIngredients.Contains(ingredient))
        {
            _addedIngredients.Add(ingredient);
            ShowShakerText($"Added {ingredient.ingredientName}");
        }
    }

    public void ShakeAndCheckCocktail()
    {
       
        if (waiting) return;
        HideShakerText();
        Debug.Log("Starting minigame");

        pendingRecipe = FindRecipe();
        Current = this;
        waiting = true;
        FishingMinigame_Shaker.Instance.StartMinigame();
        Debug.Log("[Shaker] ShakeAndCheckCocktail called");
    }
    public void HideShakerText()
    {
        if (text != null)
        {
            text.SetText("");
            text.enabled = false;
        }
    }
    void ShowShakerText(string message)
    {
        if (textRoutine != null)
            StopCoroutine(textRoutine);

        if (text != null)
        {
            text.enabled = true;
            text.SetText(message);
        }

        if (_shakerText != null)
            _shakerText.Show();

        textRoutine = StartCoroutine(HideTextAfterDelay());
    }

    CocktailRecipe FindRecipe()
    {
       
            Debug.Log($"[Shaker] Ingredients added: {_addedIngredients.Count}");

            foreach (IngredientData ing in _addedIngredients)
                Debug.Log($"[Shaker] Added ingredient: {ing.ingredientName}");

            foreach (CocktailRecipe recipe in DrinkManager.Recipes)
            {
                Debug.Log($"[Shaker] Checking recipe: {recipe.cocktailName}");

                if (Matches(recipe))
                {
                    Debug.Log($"[Shaker] match: {recipe.cocktailName}");
                    return recipe;
                }
            }

            Debug.Log("[Shaker] no match");
            return null;
        
    }

    bool Matches(CocktailRecipe recipe)
    {
        if (recipe.requiredIngredients.Length != _addedIngredients.Count)
        {
            Debug.Log($"[Shaker] {recipe.cocktailName} failed count check. Recipe needs {recipe.requiredIngredients.Length}, shaker has {_addedIngredients.Count}");
            return false;
        }

        foreach (var req in recipe.requiredIngredients)
        {
            if (req.ingredient == null)
            {
                Debug.LogWarning($"[Shaker] {recipe.cocktailName} has a null ingredient requirement.");
                return false;
            }

            if (!_addedIngredients.Contains(req.ingredient))
            {
                Debug.Log($"[Shaker] {recipe.cocktailName} missing ingredient: {req.ingredient.ingredientName}");
                return false;
            }
        }

        return true;
    }

    public void OnMinigameWin()
    {
        if (pendingRecipe != null)
        {
            //GameObject drink = Instantiate(defaultDrink, spawnPoint.position, defaultDrink.transform.rotation);
            //DrinkObject drinkScript = drink.GetComponent<DrinkObject>();
            //drinkScript.contains = pendingRecipe.cocktailName;

            //drink.transform.position += drinkSpawnOfsset;
            //drink.name = pendingRecipe.cocktailName;
            SpawnDrink(pendingRecipe.cocktailPrefab, pendingRecipe.cocktailName);
            PlayerManager.currentDrink = new Drink(pendingRecipe.cocktailName);
            Debug.Log("Correct drink spawned");
            ShowShakerText($"You made a {pendingRecipe.cocktailName} now go and serve it!");
        }
        else
        {
            SpawnDrink(failedDrinkPrefab, "Failed Drink");
            //Instantiate(failedDrinkPrefab, spawnPoint.position, spawnPoint.rotation);
            ShowShakerText("You didn't make a valid drink!");
        }

        ResetShaker();
    }

    public void OnMinigameFail()
    {
        SpawnDrink(failedDrinkPrefab, "Failed Drink");

        ShowShakerText("Failed to mix the drink");
        ResetShaker();
    }
    void SpawnDrink(GameObject prefab, string drinkName)
    {
        if (prefab == null)
        {
            Debug.LogError("[Shaker] Missing drink prefab.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[Shaker] Missing spawn point.");
            return;
        }

        GameObject drink = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

    
        drink.transform.position += drinkSpawnOfsset;
        drink.name = drinkName;

        drink.tag = "Pickup";
        drink.layer = LayerMask.NameToLayer("Pickup");

       
        if (drink.TryGetComponent(out DrinkObject drinkScript))
       {
            drinkScript.contains = drinkName;
        }

        // Make sure it can be picked up physically
        if (drink.GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"[Shaker] {drinkName} has no Collider, so it may not be pickupable.");
        }

        if (drink.GetComponent<Rigidbody>() == null)
        {
            Debug.LogWarning($"[Shaker] {drinkName} has no Rigidbody, so it may not be pickupable.");
        }
    }
    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(textDisplayTime);
        HideShakerText();
    }
    void ResetShaker()
    {
        waiting = false;
        pendingRecipe = null;
        _addedIngredients.Clear();
    }
    public void Interact()
    {
        ShakeAndCheckCocktail();
    }


}