using UnityEngine;

public class BurgerBuilder : MonoBehaviour
{
    public GameObject burgerPrefab;
    public Transform spawnPoint;

    private Burger currentBurger;

    public void CreateBurger()
    {
        GameObject burgerObj = Instantiate(burgerPrefab, spawnPoint.position, Quaternion.identity);
        currentBurger = burgerObj.GetComponent<Burger>();

        Debug.Log("Burger creada");
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (currentBurger == null) return;

        currentBurger.AddIngredient(ingredient);

        // 🔥 DESACTIVAR FÍSICA COMPLETAMENTE
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 🔥 DESACTIVAR COLLISIONES
        Collider col = ingredient.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 🔥 MOVER Y PARENT
        ingredient.transform.position = spawnPoint.position;
        ingredient.transform.SetParent(currentBurger.transform);

        // 🔥 SI ES CARNE → GUARDAR COCCIÓN
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            currentBurger.cookState = meat.currentState;
        }

        Debug.Log("Ingrediente añadido: " + ingredient.type);
    }

    void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            Debug.Log("Entró en trigger: " + ingredient.name);

            // 🔥 CREAR BURGER SI NO EXISTE
            if (currentBurger == null)
            {
                CreateBurger();
            }

            AddIngredient(ingredient);
        }
    }
}