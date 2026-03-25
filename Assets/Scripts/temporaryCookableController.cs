using UnityEngine;

public class temporaryCookableController : MonoBehaviour
{
    public float cookTime = 0.0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cooker")
            cookTime += Time.deltaTime;
    }
}
