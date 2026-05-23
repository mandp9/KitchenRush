using UnityEngine;

public class OvenController : MonoBehaviour
{
    [Header("Tiempos de Cocción (Segundos)")]
    public float timeToCook = 10f;  // Tiempo de Cruda a Cocinada
    public float timeToBurn = 20f;  // Tiempo de Cocinada a Quemada

    [Header("Estado de la Puerta")]
    public bool isDoorClosed = false;

    [SerializeField] private PizzaController currentPizza;
    [SerializeField] private float cookingTimer = 0f;
    [SerializeField] private bool isCooking = false;

    [Header("Sonidos")]
    public AudioSource cookFinishedSound;
    public AudioSource burntSound;
    public AudioSource cookingLoopSound;

    private bool hasPlayedCookedSound = false;
    private bool hasPlayedBurntSound = false;

    public GameObject efectoHumo;
    public Transform puntoAparicionHumo;

    void Update()
    {
        // Solo cocina si hay pizza, la puerta está cerrada y no está ya quemada
        if (currentPizza != null && isDoorClosed && currentPizza.cookState != PizzaCookState.Burnt)
        {
            isCooking = true;
            cookingTimer += Time.deltaTime;

            if (cookingLoopSound != null && !cookingLoopSound.isPlaying)
            {
                cookingLoopSound.Play();
                Debug.Log("Reproduciendo sonido de horno");

            }

            // Control de estados según el tiempo transcurrido
            if (currentPizza.cookState == PizzaCookState.Raw && cookingTimer >= timeToCook)
            {
                currentPizza.SetCookState(PizzaCookState.Cooked);
                if (!hasPlayedCookedSound && cookFinishedSound != null)
                {
                    cookFinishedSound.Play();
                    hasPlayedCookedSound = true;
                }
            }
            else if (currentPizza.cookState == PizzaCookState.Cooked && cookingTimer >= timeToBurn)
            {
                currentPizza.SetCookState(PizzaCookState.Burnt);
                if (!hasPlayedBurntSound && burntSound != null)
                {
                    burntSound.Play();
                    Vector3 spawnPosition = puntoAparicionHumo != null ? puntoAparicionHumo.position : transform.position;

                    GameObject humo = Instantiate(
                        efectoHumo,
                        spawnPosition,
                        Quaternion.identity
                    );

                    Destroy(humo, 4.5f);
                    hasPlayedBurntSound = true;
                }
            }
        }
        else
        {
            isCooking = false;

            if (cookingLoopSound != null && cookingLoopSound.isPlaying)
            {
                Debug.Log("Parando sonido de horno");
                cookingLoopSound.Stop();
            }
        }
    }

    public void SetDoorClosed(bool closed)
    {
        isDoorClosed = closed;
        Debug.Log(isDoorClosed ? "Puerta del horno CERRADA. Iniciando..." : " Puerta del horno ABIERTA. Cocción pausada.");
    }

    private void OnTriggerEnter(Collider other)
    {
        PizzaController pizza = other.GetComponentInParent<PizzaController>() ?? other.GetComponentInChildren<PizzaController>();

        if (pizza != null)
        {
            currentPizza = pizza;
            if (currentPizza.cookState == PizzaCookState.Raw) cookingTimer = 0f;
            else if (currentPizza.cookState == PizzaCookState.Cooked) cookingTimer = timeToCook;

            Debug.Log("Pizza colocada en el Rack del horno.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PizzaController pizza = other.GetComponentInParent<PizzaController>() ?? other.GetComponentInChildren<PizzaController>();

        if (pizza != null && pizza == currentPizza)
        {
            Debug.Log("Pizza retirada del horno.");
            currentPizza = null;
            cookingTimer = 0f;

            hasPlayedCookedSound = false;
            hasPlayedBurntSound = false;
        }


    }
}