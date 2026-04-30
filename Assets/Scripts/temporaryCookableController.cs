using UnityEngine;

public enum CookState
{
    Raw,
    Rare,
    WellDone,
    Burnt
}

public class temporaryCookableController : MonoBehaviour
{
    [Header("Cooking")]
    public float cookTime = 0f;
    public CookState currentState = CookState.Raw;

    [Header("Materials")]
    public Material rawMat;
    public Material rareMat;
    public Material wellDoneMat;
    public Material burntMat;

    [Header("Cooking Times")]
    public float rareTime = 8f;
    public float wellDoneTime = 18f;
    public float burntTime = 30f;

    [Header("Sound Effects")]
    public AudioSource cookingSound;
    public AudioSource burntSound;

    private Renderer rend;
    private CookState lastState;

    public ParticleSystem smoke;

    private bool isCooking = false; 

    void Start()
    {
        rend = GetComponent<Renderer>();
        lastState = currentState;
        UpdateMaterial();

        if (smoke != null)
            smoke.Stop();
    }

    void Update()
    {
        if (isCooking)
        {
            cookTime += Time.deltaTime;
            UpdateState();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cooker"))
        {
            isCooking = true;
            cookingSound.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cooker"))
        {
            isCooking = false;
            cookingSound.Stop();

            if (smoke != null && smoke.isPlaying)
                smoke.Stop();
        }
    }

    void UpdateState()
    {
        if (cookTime >= burntTime)
        {
            currentState = CookState.Burnt;
            cookingSound.volume = 0.2f;
        }
        else if (cookTime >= wellDoneTime)
        {
            currentState = CookState.WellDone;
            cookingSound.volume = 0.3f;
        }
        else if (cookTime >= rareTime)
        {
            currentState = CookState.Rare;
            cookingSound.volume = 0.4f;
        }
        else
        {
            currentState = CookState.Raw;
            cookingSound.volume = 0.5f;
        }

        if (currentState != lastState)
        {
            UpdateMaterial();

            if (currentState == CookState.Burnt && isCooking)
            {
                if (smoke != null && !smoke.isPlaying)
                {
                    smoke.Play();
                    burntSound.Play();
                }
            }
            else
            {
                if (smoke != null && smoke.isPlaying)
                    smoke.Stop();
            }

            lastState = currentState;
        }
    }

    void UpdateMaterial()
    {
        switch (currentState)
        {
            case CookState.Raw:
                rend.sharedMaterial = rawMat;
                break;
            case CookState.Rare:
                rend.sharedMaterial = rareMat;
                break;
            case CookState.WellDone:
                rend.sharedMaterial = wellDoneMat;
                break;
            case CookState.Burnt:
                rend.sharedMaterial = burntMat;
                break;
        }
    }

    public CookState GetCookState()
    {
        if (cookTime > burntTime) return CookState.Burnt;
        if (cookTime > wellDoneTime) return CookState.WellDone;
        if (cookTime > rareTime) return CookState.Rare;
        return CookState.Raw;
    }
}