using UnityEngine;

public enum IngredientType
{
    Meat,     // 0
    Tomato,   // 1
    Lettuce,  // 2
    Onion,    // 3
    Cheese,   // 4
    PizzaCrust, //5
    Pepperoni, //6
    Mushroom, // 7
    Olive, //8
    Ham, // 9
    Mozarella, // 10
    BaseBread = 99,
    TopBread  = 100,
    // put unimplemented ingredients here
    Ketchup,
    fries
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;
    public Vector3 customRotationOffset = Vector3.zero;

    [HideInInspector]
    public bool isPlaced = false;

}