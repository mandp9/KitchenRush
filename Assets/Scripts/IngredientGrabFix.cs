using UnityEngine;
using Valve.VR.InteractionSystem;

public class IngredientGrabFix : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    private Ingredient ingredient;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ingredient = GetComponent<Ingredient>();
    }

    // 🔥 ESTE ES EL MOMENTO CORRECTO
    private void OnAttachedToHand(Hand hand)
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (col != null)
        {
            col.enabled = true;
        }

        if (ingredient != null)
        {
            ingredient.isPlaced = false;
        }
    }
}