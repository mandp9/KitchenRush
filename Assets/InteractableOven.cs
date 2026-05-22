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
        }
        else
        {
            ObjectToRotate.localEulerAngles = closedRotation;
        }
    }

    private void HandHoverUpdate(Hand hand)
    {
        if (grabAction != null &&
            grabAction.GetStateDown(hand.handType))
        {
            ToggleDoor();
        }
    }

    private void OnMouseDown()
    {
        ToggleDoor();
    }

}