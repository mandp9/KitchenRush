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
    public float rareTime = 2f;
    public float wellDoneTime = 6f;
    public float burntTime = 10f;

    private Renderer rend;
    private CookState lastState;

    public ParticleSystem smoke;

    void Start()
    {
        rend = GetComponent<Renderer>();
        lastState = currentState;
        UpdateMaterial();
        if (smoke != null)
            smoke.Stop();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cooker"))
        {
            cookTime += Time.deltaTime;
            UpdateState();
        }
    }

    void UpdateState()
    {
        if (cookTime >= burntTime)
            currentState = CookState.Burnt;
        else if (cookTime >= wellDoneTime)
            currentState = CookState.WellDone;
        else if (cookTime >= rareTime)
            currentState = CookState.Rare;
        else
            currentState = CookState.Raw;

        if (currentState != lastState)
        {
            UpdateMaterial();

            if (currentState == CookState.Burnt)
            {
                if (smoke != null && !smoke.isPlaying)
                    smoke.Play();
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
}