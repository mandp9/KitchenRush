using UnityEngine;

public enum IngredientType
{
    Meat,     // 0
    Tomato,   // 1
    Lettuce,  // 2
    Onion,    // 3
    Cheese,   // 4
    BaseBread = 99,
    TopBread  = 100,
    // put unimplemented ingredients here
    Ketchup,
    fries
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;

    [HideInInspector]
    public bool isPlaced = false;
}