using UnityEngine;

public class KetchupController : MonoBehaviour
{
    public AudioSource soundSource;
    public AudioClip[] soundClips;

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>().type == IngredientType.BaseBread)
        {
            soundSource.clip = soundClips[Random.Range(0, soundClips.Length)];
            soundSource.Play();
        }
    }
}
