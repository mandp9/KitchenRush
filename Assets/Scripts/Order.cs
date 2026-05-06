using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public CookState requiredCookState;
    public List<IngredientType> requiredIngredients;

    public bool requiresFries;
    public bool requiresDrink; // futuro
}