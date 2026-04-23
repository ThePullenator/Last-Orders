using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

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
            _shakerText.Show();
            text.SetText($"Added {ingredient.ingredientName}");
        }
    }

    public void ShakeAndCheckCocktail()
    {
        if (waiting) return;

        Debug.Log("Starting minigame");

        pendingRecipe = FindRecipe();
        Current = this;
        waiting = true;

        FishingMinigame_Shaker.Instance.StartMinigame();
        Debug.Log("[Shaker] ShakeAndCheckCocktail called");
    }

    CocktailRecipe FindRecipe()
    {
        foreach (CocktailRecipe recipe in DrinkManager.Recipes)
        {
            if (Matches(recipe))
                return recipe;
        }
        return null;
    }

    bool Matches(CocktailRecipe recipe)
    {
        if (recipe.requiredIngredients.Length != _addedIngredients.Count)
            return false;

        foreach (var req in recipe.requiredIngredients)
            if (!_addedIngredients.Contains(req.ingredient))
                return false;

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

            PlayerManager.currentDrink = new Drink(pendingRecipe.cocktailName);
            Debug.Log("Correct drink spawned");
            _shakerText.Show();
            text.SetText($"You made a {pendingRecipe.cocktailName} now go and serve it!");
        }
        else
        {
            //Instantiate(failedDrinkPrefab, spawnPoint.position, spawnPoint.rotation);
            _shakerText.Show();
            text.SetText("You didn't make a valid drink!");
        }

        ResetShaker();
    }

    public void OnMinigameFail()
    {
        //Instantiate(failedDrinkPrefab, spawnPoint.position, spawnPoint.rotation);
        _shakerText.Show();
        text.SetText($"Failed to mix the drink");

        ResetShaker();
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