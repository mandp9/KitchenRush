using UnityEngine;

public enum FryState
{
    Raw,
    Cooked
}

public class FriesCooker : MonoBehaviour
{
    [Header("Cooking")]
    public float cookTime = 0f;
    public float cookDuration = 5f;

    public FryState currentState = FryState.Raw;

    [Header("Materials")]
    public Material rawMat;
    public Material cookedMat;

    private Renderer rend;
    private bool isCooking = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateMaterial();
    }

    void Update()
    {
        if (!isCooking) return;

        cookTime += Time.deltaTime;

        if (cookTime >= cookDuration && currentState == FryState.Raw)
        {
            currentState = FryState.Cooked;
            UpdateMaterial();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oil"))
        {
            isCooking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oil"))
        {
            isCooking = false;
        }
    }

    void UpdateMaterial()
    {
        if (currentState == FryState.Raw)
            rend.material = rawMat;
        else
            rend.material = cookedMat;
    }

    public bool IsCooked()
    {
        return currentState == FryState.Cooked;
    }
}