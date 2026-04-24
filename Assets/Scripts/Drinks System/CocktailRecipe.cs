using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public IngredientData ingredient;
}

[CreateAssetMenu(fileName = "NewCocktail", menuName = "Cocktail/Recipe")]
public class CocktailRecipe : ScriptableObject
{
    public string cocktailName; 
    public IngredientRequirement[] requiredIngredients; 
    public DrinkEffectContainer[] effects;
    [Header("Spawn")]
    public GameObject cocktailPrefab;
    [Header("Recipe Book")]
    [TextArea(3, 8)]
    public string instructions;

    public Sprite recipeSprite;
}