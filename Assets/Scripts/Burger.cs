using System.Collections.Generic;
using UnityEngine;

public class Burger : MonoBehaviour
{
    public CookState cookState;
    public List<IngredientType> ingredients = new List<IngredientType>();

    public void AddIngredient(Ingredient ingredient)
    {
        if (!ingredients.Contains(ingredient.type))
        {
            ingredients.Add(ingredient.type);
        }
    }
}