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

        Collider col = ingredient.GetComponent<Collider>();
        float height = heightStep;

        if (col != null)
        {
            height = col.bounds.size.y;
        }

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 🔥 Parent + posición local (STACKING)
        ingredient.transform.SetParent(currentBurger.transform);
        ingredient.transform.localPosition = new Vector3(0, currentHeight, 0);
        ingredient.transform.localRotation = Quaternion.identity;

        // 🔥 Desactivar collider después
        if (col != null)
        {
            col.enabled = false;
        }

        currentHeight += height;

        // 🔥 Guardar cocción
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            currentBurger.cookState = meat.currentState;
        }

        Debug.Log("Ingrediente añadido: " + ingredient.type);
    }

    void OnCollisionStay(Collision collision)
    {
        Ingredient ingredient = collision.gameObject.GetComponent<Ingredient>();

        if (ingredient != null && !ingredient.isPlaced)
        {
            Debug.Log("Colisión con: " + ingredient.name);

            if (currentBurger == null)
            {
                CreateBurger();
            }

            AddIngredient(ingredient);

            ingredient.isPlaced = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Ingredient ingredient = collision.gameObject.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            Debug.Log("Salió de la tabla: " + ingredient.name);
        }
    }
}