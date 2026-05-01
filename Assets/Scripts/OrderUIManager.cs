using UnityEngine;
using TMPro;

public class OrderUIManager : MonoBehaviour
{
    [Header("UI")]
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
            Debug.Log("Slot " + i + " ocupado: " + ocupado[i]);

            if (!ocupado[i] || string.IsNullOrEmpty(orderTexts[i].text))
            {
                ocupado[i] = true;
                orderTexts[i].text = texto;


                if (audioSource != null && takingOrderSound != null)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(takingOrderSound, 4f);
                }

                return i;
            }
        }

        Debug.LogWarning("❌ No hay hueco para más pedidos");
        return -1;
    }

    public void LiberarPedido(int index)
    {
        if (index < 0 || index >= orderTexts.Length)
        {
            return;
        }

        Debug.Log("🧹 Liberando slot " + index);

        ocupado[index] = false;
        orderTexts[index].text = "";

        if (audioSource != null && crossOutSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(crossOutSound, 4f);
        }
    }
}