using UnityEngine;
using Valve.VR.InteractionSystem;

public enum IngredientType
{
    Meat,
    Lettuce,
    Tomato,
    Onion,
    Cheese,
    BaseBread,
    TopBread
}

public class Ingredient : MonoBehaviour
{
    public IngredientType type;

    [HideInInspector]
    public bool isPlaced = false;

    public GameObject burgerPrefab;

    void OnDetachedFromHand(Hand hand)
    {
        // 🔥 SOLO si es pan base
        if (type == IngredientType.BaseBread && !isPlaced)
        {
            Vector3 spawnPos = transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
            {
                spawnPos.y = hit.point.y;
            }

            GameObject burgerGO = Instantiate(burgerPrefab, spawnPos, Quaternion.identity);

            BurgerBuilder builder = burgerGO.GetComponent<BurgerBuilder>();

            if (builder != null)
            {
                builder.AddIngredient(this); // 🔥 añade el pan directamente
            }
        }
    }
}