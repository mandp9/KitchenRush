using UnityEngine;
using Valve.VR.InteractionSystem;

public class InfiniteIngredient : MonoBehaviour
{
    [HideInInspector]
    public InfiniteIngredientSpawner originSpawner;

    private bool hasTriggered = false;

    void OnAttachedToHand(Hand hand)
    {
        // 🔥 evitar duplicaciones múltiples
        if (hasTriggered) return;

        hasTriggered = true;

        if (originSpawner != null)
        {
            originSpawner.OnItemTaken();

            // 🔥 IMPORTANTE: romper vínculo
            originSpawner = null;
        }
    }
}