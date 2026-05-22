using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections;

public class FriesContainerPickup : MonoBehaviour
{
    public GameObject fullFriesPrefab;

    private bool puedeLlenarse = false;

    void Start()
    {
        Invoke(nameof(Activar), 0.25f);
    }

    void Activar()
    {
        puedeLlenarse = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!puedeLlenarse)
            return;

        FriesCooker fries =
            other.GetComponentInParent<FriesCooker>();

        if (fries == null)
            return;

        if (!fries.IsCooked())
            return;

        puedeLlenarse = false;

        Hand hand =
            GetComponentInParent<Hand>();

        GameObject newFries =
            Instantiate(
                fullFriesPrefab,
                transform.position,
                transform.rotation
            );

        Debug.Log(
            "Nuevo objeto: " +
            newFries.name
        );

        Debug.Log(
            "Tiene interactable: " +
            (newFries.GetComponent<Interactable>() != null)
        );

        if (hand != null)
        {
            hand.DetachObject(
                gameObject,
                false
            );

            Rigidbody rb =
                GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity =
                    Vector3.zero;

                rb.angularVelocity =
                    Vector3.zero;
            }

            Interactable newInteractable =
                newFries.GetComponent<Interactable>();

            if (newInteractable != null)
            {
                StartCoroutine(
                    AttachDelayed(
                        hand,
                        newFries
                    )
                );
            }
        }

        Collider[] friesCols =
            fries.GetComponentsInChildren<Collider>();

        foreach (Collider c in friesCols)
        {
            c.enabled = false;
        }

        Renderer[] friesRenderers =
            fries.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in friesRenderers)
        {
            r.enabled = false;
        }

        Destroy(
            fries.gameObject,
            0.5f
        );

        Interactable interact =
            GetComponent<Interactable>();

        if (interact != null)
        {
            interact.enabled = false;
        }

        Collider[] myCols =
            GetComponentsInChildren<Collider>();

        foreach (Collider c in myCols)
        {
            c.enabled = false;
        }

        Renderer[] myRenderers =
            GetComponentsInChildren<Renderer>();

        foreach (Renderer r in myRenderers)
        {
            r.enabled = false;
        }
        
        Destroy(
            gameObject,
            0.5f
        );
    }

    IEnumerator AttachDelayed(
        Hand hand,
        GameObject obj
    )
    {
        yield return null;

        if (hand != null)
        {
            hand.AttachObject(
                obj,
                GrabTypes.Grip
            );
        }
    }
}