using UnityEngine;

public enum IngredientType
{
    Meat,
    Lettuce,
    Tomato,
    Onion,
    Cheese,
    BaseBread,
    TopBread,
    fries
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;

    [HideInInspector]
    public bool isPlaced = false;
}