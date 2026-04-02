using UnityEngine;

public enum IngredientType
{
    TopBread,
    BaseBread,
    Meat,
    Lettuce,
    Tomato,
    Onion,
    Cheese
}
public class Ingredient : MonoBehaviour
{
    public IngredientType type;
}
