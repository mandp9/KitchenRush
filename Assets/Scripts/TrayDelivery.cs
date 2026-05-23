using UnityEngine;

public class TrayDelivery : MonoBehaviour
{
    public MonoBehaviour npcActual;

    void OnTriggerEnter(Collider other)
    {
        if (npcActual == null)
            return;

        BurgerController burger = other.GetComponent<BurgerController>();

        if (burger != null)
        {
            if (npcActual is NPCClient npc)
            {
                npc.RecibirBurger(burger);
            }

            if (npcActual is NPCClientOldLady oldLady)
            {
                oldLady.RecibirBurger(burger);
            }
            return;
        }

        Drink drink = other.GetComponent<Drink>();

        if (drink != null)
        {
            if (npcActual is NPCClient npc)
            {
                npc.RecibirDrink(drink);
            }

            if (npcActual is NPCClientOldLady oldLady)
            {
                oldLady.RecibirDrink(drink);
            }
            return;
        }

        if (other.transform.root.name.Contains("frieswithcontainer"))
        {
            if (npcActual is NPCClient npc)
            {
                npc.RecibirFries(other.transform.root.gameObject);
            }

            if (npcActual is NPCClientOldLady oldLady)
            {
                oldLady.RecibirFries(other.transform.root.gameObject);
            }

            return;
        }

        if (other.transform.root.name.Contains("Kebab_Wrap"))
        {
            if (npcActual is NPCClient npc)
            {
                npc.RecibirKebab(other.transform.root.gameObject);
            }
            return;
        }
    }

    public void LimpiarBandeja()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        Collider[] objetos = Physics.OverlapBox(
            box.bounds.center,
            box.bounds.extents,
            transform.rotation
        );

        foreach (Collider col in objetos)
        {
            // ignorar propia bandeja
            if (col.transform == transform)
                continue;

            // ignorar hijos de la bandeja
            if (col.transform.IsChildOf(transform))
                continue;

            Destroy(col.transform.root.gameObject);
        }
    }
}