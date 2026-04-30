using UnityEngine;
using System.Collections.Generic;

public class OilFryer : MonoBehaviour
{
    public AudioSource fryingAudio;

    private HashSet<GameObject> ingredientsInside = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<Ingredient>() != null)
        {
            ingredientsInside.Add(other.gameObject);

            if (!fryingAudio.isPlaying)
                fryingAudio.Play();
                Debug.Log("Sonido ON");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ingredient>() != null)
        {
            ingredientsInside.Remove(other.gameObject);

            if (ingredientsInside.Count == 0)
                fryingAudio.Stop();
        }
    }
}