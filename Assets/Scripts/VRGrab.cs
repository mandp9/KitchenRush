using UnityEngine;
using Valve.VR;

public class VRGrab : MonoBehaviour
{
    public Transform holdPoint;
    public float grabDistance = 3f;

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabAction;

    private GameObject heldObject;

    void Update()
    {
        // VR
        if (grabAction.GetStateDown(handType))
        {
            if (heldObject == null)
                TryGrab();
            else
                Drop();
        }

        // Teclado
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
                TryGrab();
            else
                Drop();
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                heldObject = hit.collider.gameObject;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();

                // 🔥 Estado: en mano
                rb.isKinematic = true;
                rb.useGravity = false;

                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void Drop()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        // 🔥 Estado: física activa
        rb.isKinematic = false;
        rb.useGravity = true;

        heldObject.transform.SetParent(null);

        heldObject = null;
    }
}