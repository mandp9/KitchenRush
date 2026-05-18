using UnityEngine;
using Seagull.Interior_I1.SceneProps;

public class DoorInteraction : MonoBehaviour
{
    private Rotatable rotatable;

    private bool abierta = false;

    private float objetivo = 0f;

    public float velocidad = 2f;

    void Start()
    {
        rotatable = GetComponent<Rotatable>();
    }

    void Update()
    {
        rotatable.rotation = Mathf.Lerp(
            rotatable.rotation,
            objetivo,
            Time.deltaTime * velocidad
        );
    }

    // RATÓN
    void OnMouseDown()
    {
        ToggleDoor();
    }

    // VR
    public void ToggleDoor()
    {
        abierta = !abierta;

        objetivo = abierta ? 1f : 0f;
    }
}