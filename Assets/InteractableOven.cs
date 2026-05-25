using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class InteractableOven : MonoBehaviour
{
    private bool isOpen;

    public Vector3 openRotation;
    public Vector3 closedRotation;
    public Transform ObjectToRotate;

    [Header("Referencia al Horno")]
    [Tooltip("Arrastra aquí el objeto padre que tiene el script OvenController")]
    public OvenController ovenController;

    public SteamVR_Action_Boolean grabAction;

    void Start()
    {
        UpdateDoorState();
    }

    void ToggleDoor()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        UpdateDoorState();
    }

    void CloseDoor()
    {
        isOpen = false;
        UpdateDoorState();
    }

    void UpdateDoorState()
    {
        if (isOpen)
        {
            ObjectToRotate.localEulerAngles = openRotation;

            if (ovenController != null)
            {
                ovenController.SetDoorClosed(false);
            }
        }
        else
        {
            ObjectToRotate.localEulerAngles = closedRotation;

            if (ovenController != null)
            {
                ovenController.SetDoorClosed(true);
            }
        }
    }

    private void HandHoverUpdate(Hand hand)
    {
        SteamVR_Input_Sources handType = SteamVR_Input_Sources.Any;
        if (SteamVR_Input.GetStateDown("GrabPinch", handType))
        {
            ToggleDoor();
        }
    }

    private void OnMouseDown()
    {
        ToggleDoor();
    }
}