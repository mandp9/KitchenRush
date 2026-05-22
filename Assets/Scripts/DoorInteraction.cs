using UnityEngine;
using Seagull.Interior_I1.SceneProps;

public class DoorInteraction : MonoBehaviour
{
    private Rotatable rotatable;
    private bool abierta = false;
    private float objetivo = 0f;

    public float velocidad = 3f; 

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool sonidoCierrePendiente = false;

    private float cooldownTiempo = 1.2f; 
    private float siguienteClickPermitido = 0f;

    void Start()
    {
        rotatable = GetComponent<Rotatable>();
    }

    void Update()
    {
        rotatable.rotation = Mathf.MoveTowards(
            rotatable.rotation,
            objetivo,
            Time.deltaTime * velocidad
        );

        if (sonidoCierrePendiente && Mathf.Abs(rotatable.rotation) < 0.02f)
        {
            sonidoCierrePendiente = false;

            if (audioSource != null && closeSound != null)
            {
                audioSource.pitch = Random.Range(0.97f, 1.03f);
                audioSource.PlayOneShot(closeSound);
            }
        }
    }

    void OnMouseDown()
    {
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        if (Time.time < siguienteClickPermitido) return;

        siguienteClickPermitido = Time.time + cooldownTiempo;

        abierta = !abierta;
        objetivo = abierta ? 1f : 0f;

        if (abierta)
        {
            sonidoCierrePendiente = false;
            
            if (audioSource != null && openSound != null)
            {
                audioSource.pitch = Random.Range(0.97f, 1.03f);
                audioSource.PlayOneShot(openSound);
            }
        }
        else
        {
            sonidoCierrePendiente = true;
        }
    }
}