using UnityEngine;

public class BurgerBuilder : MonoBehaviour
{
    public GameObject burgerPrefab;
    public Transform spawnPoint;

    private Burger currentBurger;

    private float currentHeight = 0f;
    public float heightStep = 0.05f;

    private bool burgerStarted = false;
    private bool burgerFinished = false;

    void OnCollisionStay(Collision collision)
    {
        Ingredient ingredient = collision.gameObject.GetComponent<Ingredient>();

        if (ingredient == null || ingredient.isPlaced) return;

        // ❌ si no hay burger y no es pan base → ignorar
        if (!burgerStarted && ingredient.type != IngredientType.BaseBread)
            return;

        // 🍞 PAN BASE → INICIAR BURGER
        if (!burgerStarted && ingredient.type == IngredientType.BaseBread)
        {
            CreateBurger(ingredient);

            AddIngredient(ingredient);

            burgerStarted = true;
            ingredient.isPlaced = true;

            Debug.Log("Burger iniciada");
            return;
        }

        // ❌ si ya terminó → ignorar
        if (burgerFinished) return;

        // 🍞 PAN TOP → FINALIZAR
        if (ingredient.type == IngredientType.TopBread)
        {
            AddIngredient(ingredient);

            burgerFinished = true;
            ingredient.isPlaced = true;

            LockBurger();

            Debug.Log("Burger terminada");
            return;
        }

        // 🥩 OTROS INGREDIENTES
        AddIngredient(ingredient);
        ingredient.isPlaced = true;
    }

    // 🔥 MUY IMPORTANTE: crear burger en la posición REAL del pan
    public void CreateBurger(Ingredient baseIngredient)
    {
        Vector3 pos = baseIngredient.transform.position;

        GameObject burgerObj = Instantiate(burgerPrefab, pos, Quaternion.identity);
        currentBurger = burgerObj.GetComponent<Burger>();

        currentHeight = 0f;
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

        float halfHeight = height / 2f;

        // 🔥 IMPORTANTE: usar POSICIÓN LOCAL CORRECTA
        ingredient.transform.SetParent(currentBurger.transform);

        ingredient.transform.localPosition = new Vector3(
            0,
            currentHeight + halfHeight,
            0
        );

        ingredient.transform.localRotation = Quaternion.identity;

        currentHeight += height;

        // cook state
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            currentBurger.cookState = meat.currentState;
        }

        Debug.Log("Ingrediente añadido: " + ingredient.type);
    }

    void LockBurger()
    {
        foreach (Transform child in currentBurger.transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Collider col = child.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }

        Debug.Log("Burger bloqueada");
    }

    public void ResetBurger()
    {
        currentBurger = null;
        burgerStarted = false;
        burgerFinished = false;
        currentHeight = 0f;
    }
}