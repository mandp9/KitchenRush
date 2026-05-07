using UnityEngine;

public enum IngredientType
{
    Meat,     // 0
    Tomato,   // 1
    BaseBread = 99,
    TopBread  = 100,
    // put unimplemented ingredients here
    Lettuce,
    Onion,
    Cheese,
    Ketchup,
    fries
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;

    [HideInInspector]
    public bool isPlaced = false;
}