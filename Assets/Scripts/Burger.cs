using System.Collections.Generic;
using UnityEngine;

public class Burger : MonoBehaviour
{
    public CookState cookState;

    public List<IngredientType> ingredients = new List<IngredientType>();

    public List<Ingredient> ingredientObjects = new List<Ingredient>();

    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient.type);

        ingredientObjects.Add(ingredient);
    }
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredients.Contains(ingredient.type))
        {
            ingredients.Remove(ingredient.type);
            ingredientObjects.Remove(ingredient);
        }
    }
}