using UnityEngine;
using UnityEngine.SceneManagement;

public class BellStartGame : MonoBehaviour
{
    [Header("Configuración")]
    public int levelToLoad = 1;
    public float cooldown = 1f;          // evita que se dispare varias veces

    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip bellSound;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Solo reacciona a los controladores
        if (triggered) return;
        if (!other.CompareTag("GameController")) return;

        triggered = true;

        // Sonido
        if (audioSource && bellSound)
            audioSource.PlayOneShot(bellSound);

        // Cargar nivel tras un pequeño delay (para que suene el audio)
        Invoke(nameof(LoadLevel), cooldown);
    }

    void LoadLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}