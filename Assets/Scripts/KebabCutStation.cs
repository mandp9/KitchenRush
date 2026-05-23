using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem; 

[RequireComponent(typeof(Collider))]
public class KebabCutStation : MonoBehaviour
{
    [Header("Cutting Setup")]
    public int requiredCuts = 5;
    public float cutCooldown = 0.5f; 

    [Header("Results")]
    public GameObject wrapPrefab; 

    [Header("Knife Setup")]
    public GameObject realKnifeObject; 
    public Transform knifeReturnTarget; 

    [Header("Color Code Effects")]
    public Renderer meatRenderer; // 
    public Color originalColor = new Color(0.45f, 0.25f, 0.15f); 
    public Color rawMeatColor = new Color(0.85f, 0.2f, 0.2f);    

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip cutSound;
    public AudioClip completedSound;

    private int currentCuts = 0;
    private float nextCutAllowedTime = 0f;
    private bool isStationCompleted = false;

    void Start()
    {
        if (meatRenderer == null)
        {
            meatRenderer = GetComponentInChildren<Renderer>();
        }

        if (meatRenderer != null && meatRenderer.materials.Length > 1)
        {
            meatRenderer.materials[1].color = originalColor;
        }
        else
        {
            Debug.LogError("No se encontró ningún Renderer o el objeto no tiene un segundo material para la carne.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStationCompleted || Time.time < nextCutAllowedTime) return;

        KnifeCollisionDetector knife = other.GetComponent<KnifeCollisionDetector>();
        if (knife != null)
        {
            ProcessCut(other);
        }
    }

    void ProcessCut(Collider knifeCollider)
    {
        nextCutAllowedTime = Time.time + cutCooldown;
        currentCuts++;

        float cutPercentage = (float)currentCuts / requiredCuts;
        
        if (meatRenderer != null && meatRenderer.materials.Length > 1)
        {
            meatRenderer.materials[1].color = Color.Lerp(originalColor, rawMeatColor, cutPercentage);
        }

        PlaySound(cutSound);

        if (currentCuts >= requiredCuts)
        {
            FinishStation(knifeCollider);
        }
    }

    void FinishStation(Collider knifeCollider)
    {
        isStationCompleted = true;
        PlaySound(completedSound);
        StartCoroutine(SpawnWrapInPlayerHand(knifeCollider));
    }

    IEnumerator SpawnWrapInPlayerHand(Collider knifeCollider)
    {
        GameObject knifeToMove = realKnifeObject != null ? realKnifeObject : knifeCollider.gameObject;
        
        Hand handHoldingKnife = knifeCollider.GetComponentInParent<Hand>();
        if (handHoldingKnife == null && realKnifeObject != null)
        {
            handHoldingKnife = realKnifeObject.GetComponentInParent<Hand>();
        }

        if (handHoldingKnife == null)
        {
            Debug.LogError("SteamVR Hand not found holding the knife.");
            Instantiate(wrapPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
            ResetStation();
            yield break;
        }

        handHoldingKnife.DetachObject(knifeToMove);
        handHoldingKnife.HoverLock(null); 

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Rigidbody knifeRb = knifeToMove.GetComponent<Rigidbody>();
        if (knifeRb == null) knifeRb = knifeToMove.GetComponentInChildren<Rigidbody>();

        if (knifeRb != null)
        {
            knifeRb.isKinematic = true; 
            knifeRb.linearVelocity = Vector3.zero;
            knifeRb.angularVelocity = Vector3.zero;
        }

        if (knifeReturnTarget != null)
        {
            knifeToMove.transform.position = knifeReturnTarget.position;
            knifeToMove.transform.rotation = knifeReturnTarget.rotation;
        }

        GameObject newWrap = Instantiate(wrapPrefab);
        newWrap.name = "Kebab_Wrap";

        yield return new WaitForEndOfFrame(); 
        handHoldingKnife.AttachObject(newWrap, GrabTypes.Grip);

        yield return new WaitForSeconds(2f);
        ResetStation();
    }

    void ResetStation()
    {
        currentCuts = 0;
        isStationCompleted = false;
        
        if (meatRenderer != null && meatRenderer.materials.Length > 1)
        {
            meatRenderer.materials[1].color = originalColor;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }
}