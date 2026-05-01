using UnityEngine;
using TMPro;

public class OrderUIManager : MonoBehaviour
{
    public TextMeshProUGUI[] orderTexts;

    private bool[] ocupado;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip takingOrderSound;
    public AudioClip crossOutSound;

    void Awake()
    {
        ocupado = new bool[orderTexts.Length];

        for (int i = 0; i < orderTexts.Length; i++)
        {
            orderTexts[i].text = "";
            ocupado[i] = false;
        }
    }

    public int AsignarPedido(string texto)
    {
        for (int i = 0; i < orderTexts.Length; i++)
        {
            if (!ocupado[i])
            {
                ocupado[i] = true;
                orderTexts[i].text = texto;

                if (audioSource != null && takingOrderSound != null)
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(takingOrderSound, 3f);

                return i;
            }
        }

        Debug.LogWarning("No hay hueco para más pedidos");
        return -1;
    }

    public void LiberarPedido(int index)
    {
        if (index < 0 || index >= orderTexts.Length) return;

        ocupado[index] = false;
        orderTexts[index].text = "";

        if (audioSource != null && crossOutSound != null)
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(crossOutSound, 2f);
    }
}