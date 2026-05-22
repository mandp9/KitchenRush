using UnityEngine;
using Valve.VR.InteractionSystem;

public class InfiniteIngredient : MonoBehaviour
{
    [HideInInspector]
    public InfiniteIngredientSpawner originSpawner;

    [HideInInspector]
    public InfiniteChipsSpawner chipsSpawner;

    private bool hasTriggered = false;

    void OnAttachedToHand(Hand hand)
    {
        transform.SetParent(null, true);

        if (hasTriggered)
            return;

        hasTriggered = true;

        if (originSpawner != null)
        {
            originSpawner.OnItemTaken();
            originSpawner = null;
        }

        if (chipsSpawner != null)
        {
            chipsSpawner.OnItemTaken();
            chipsSpawner = null;
        }
    }
}