using UnityEngine;

public class BurgerBuilder : MonoBehaviour
{
    public GameObject burgerPrefab;
    public Transform spawnPoint;

    private Burger currentBurger;

    private float currentHeight = 0f;
    public float heightStep = 0.05f;

    public void CreateBurger()
    {
        GameObject burgerObj = Instantiate(burgerPrefab, spawnPoint.position, Quaternion.identity);
        currentBurger = burgerObj.GetComponent<Burger>();

        currentHeight = 0f;

        Debug.Log("Burger creada");
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (currentBurger == null) return;

        currentBurger.AddIngredient(ingredient);

        // 🔥 OBTENER COLLIDER
        Collider col = ingredient.GetComponent<Collider>();
        float height = heightStep;

        if (col != null)
        {
            height = col.bounds.size.y;
        }

        // 🔥 DESACTIVAR FÍSICA
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 🔥 HACER HIJO Y POSICIONAR (STACKING)
        ingredient.transform.SetParent(currentBurger.transform);

        Vector3 localPos = new Vector3(0, currentHeight, 0);
        ingredient.transform.localPosition = localPos;

        // 🔥 RESET ROTACIÓN
        ingredient.transform.localRotation = Quaternion.identity;

        // 🔥 DESACTIVAR COLLIDER (después de calcular altura)
        if (col != null)
        {
            col.enabled = false;
        }

        // 🔥 AUMENTAR ALTURA
        currentHeight += height;

        // 🔥 SI ES CARNE → GUARDAR COCCIÓN
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            currentBurger.cookState = meat.currentState;
        }

        Debug.Log("Ingrediente añadido: " + ingredient.type);
    }

    void OnTriggerStay(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null && !ingredient.isPlaced)
        {
            Debug.Log("Entró en trigger: " + ingredient.name);

            if (currentBurger == null)
            {
                CreateBurger();
            }

            AddIngredient(ingredient);

            ingredient.isPlaced = true;
        }
    }
}