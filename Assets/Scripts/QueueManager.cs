using UnityEngine;
using System.Collections.Generic;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;

    public List<NPCClient> cola = new();

    void Awake()
    {
        instance = this;
    }

    public void ActualizarCola()
    {
        ActualizarPosiciones();
    }

    public void EntrarCola(NPCClient npc)
    {
        cola.Add(npc);

        ActualizarPosiciones();
    }

    public void SalirCola(NPCClient npc)
    {
        cola.Remove(npc);

        ActualizarPosiciones();
    }

    void ActualizarPosiciones()
    {
        for (int i = 0; i < cola.Count; i++)
        {
            cola[i].posicionCola = i;
            cola[i].ActualizarPosicionCola();
        }

        if (cola.Count > 0)
        {
            cola[0].IntentarIrABarra();
        }
    }
}