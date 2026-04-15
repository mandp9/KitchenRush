using UnityEngine;
using Valve.VR;

public class VRGrab : MonoBehaviour
{
    public Transform holdPoint;
    public float grabDistance = 3f;

    public AudioSource grabSound;
    public AudioSource dropSound;

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabAction;

    private GameObject heldObject;

    void Update()
    {
        if (grabAction != null && grabAction.GetStateDown(handType))
        {
            ToggleGrab();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleGrab();
        }

        if (Input.GetMouseButtonDown(0))
        {
            ToggleGrab();
        }
    }

    void ToggleGrab()
    {
        if (heldObject == null)
            TryGrab();
        else
            Drop();
    }

    void TryGrab()
    {
        Ray ray;

        if (Camera.main != null)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = new Ray(transform.position, transform.forward);
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                grabSound.Play();

                heldObject = hit.collider.gameObject;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = new Vector3(0, -0.1f, 0.4f);
                heldObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void Drop()
    {
        if (heldObject == null) return;

        dropSound.Play();

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        heldObject.transform.SetParent(null);
        heldObject = null;
    }
}