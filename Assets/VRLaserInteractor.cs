using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class VRLaserInteractor : MonoBehaviour
{
    public SteamVR_Action_Boolean clickAction = SteamVR_Input.GetBooleanAction("InteractUI");
    public SteamVR_Input_Sources hand = SteamVR_Input_Sources.RightHand;

    [Header("Laser visual")]
    public float rayDistance = 5f;
    public LineRenderer lineRenderer;

    private Button hoveredButton;

    void Update()
    {
        // Lanzar rayo desde el controlador
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Dibujar laser
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
        }

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Button btn = hit.collider.GetComponentInParent<Button>();

            // Hover
            if (btn != null && btn != hoveredButton)
            {
                if (hoveredButton != null)
                    hoveredButton.targetGraphic.color = hoveredButton.colors.normalColor;

                hoveredButton = btn;
                hoveredButton.targetGraphic.color = hoveredButton.colors.highlightedColor;

                // Acortar laser hasta el botón
                if (lineRenderer != null)
                    lineRenderer.SetPosition(1, hit.point);
            }

            // Click con gatillo
            if (btn != null && clickAction.GetStateDown(hand))
            {
                btn.onClick.Invoke();
            }
        }
        else
        {
            // Sin hover
            if (hoveredButton != null)
            {
                hoveredButton.targetGraphic.color = hoveredButton.colors.normalColor;
                hoveredButton = null;
            }
        }
    }
}