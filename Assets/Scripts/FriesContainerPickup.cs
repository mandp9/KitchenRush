using UnityEngine;
using Valve.VR.InteractionSystem;

public class FriesContainerPickup : MonoBehaviour
{
    public GameObject fullFriesPrefab;

    private void OnTriggerEnter(Collider other)
    {
        FriesCooker fries = other.GetComponentInParent<FriesCooker>();

        if (fries == null || !fries.IsCooked()) return;

        Debug.Log("🍟 Patatas recogidas");

        Hand hand = GetComponentInParent<Hand>();
        Interactable interactable = GetComponent<Interactable>();

        GameObject newFries = Instantiate(fullFriesPrefab, transform.position, transform.rotation);

        if (hand != null && interactable != null)
        {
            // 🔥 forma correcta
            hand.DetachObject(gameObject, false);

            // 🔥 importante: resetear velocidad
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = Vector3.zero;

            // 🔥 coger nuevo objeto
            Interactable newInteractable = newFries.GetComponent<Interactable>();
            if (newInteractable != null)
                hand.AttachObject(newFries, GrabTypes.Grip);
        }

        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}