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

    // RESERVAR SLOT
    public int ReservarSlot()
    {
        for (int i = 0; i < ocupado.Length; i++)
        {
            if (!ocupado[i])
            {
                ocupado[i] = true;

                Debug.Log("✅ Slot reservado: " + i);

                return i;
            }
        }

        return -1;
    }

    // ESCRIBIR PEDIDO EN SLOT
    public void EscribirPedido(int slot, string texto)
    {
        if (slot < 0 || slot >= orderTexts.Length)
            return;

        orderTexts[slot].text = texto;

        if (audioSource != null && takingOrderSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(takingOrderSound, 4f);
        }
    }

    // LIBERAR SLOT
    public void LiberarPedido(int index)
    {
        if (index < 0 || index >= orderTexts.Length)
            return;

        Debug.Log("🧹 Liberando slot " + index);

        ocupado[index] = false;

        orderTexts[index].text = "";

        if (audioSource != null && crossOutSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(crossOutSound, 4f);
        }
    }

    // OPCIONAL
    public bool HayHuecoLibre()
    {
        for (int i = 0; i < ocupado.Length; i++)
        {
            if (!ocupado[i])
                return true;
        }

        return false;
    }
}