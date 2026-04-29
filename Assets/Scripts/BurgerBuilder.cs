using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;

public class BurgerBuilder : MonoBehaviour
{
    private float currentHeight = 0f;
    public float heightStep = 0.05f;
    private float ignoreExitTimer = 0f;

    [Header("Estado de la Hamburguesa")]
    public List<Ingredient> stackedIngredients = new List<Ingredient>();
    public CookState finalCookState;

    private Burger burger;

    private void Start()
    {
        burger = GetComponent<Burger>();

        Collider col = GetComponent<Collider>();
        if (col != null)
            currentHeight = col.bounds.extents.y * 2f;
        else
            currentHeight = heightStep;
    }

    private void Update()
    {
        if (ignoreExitTimer > 0) ignoreExitTimer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null || ingredient.isPlaced || ingredient.gameObject == this.gameObject)
            return;

        // 🔥 SOLO empezar con pan base
        if (stackedIngredients.Count == 0 && ingredient.type != IngredientType.BaseBread)
            return;

        Interactable interactable = ingredient.GetComponentInParent<Interactable>();
        if (interactable != null && interactable.attachedToHand != null)
            return;

        AddIngredient(ingredient);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        Debug.Log("➕ Añadiendo: " + ingredient.type);

        ingredient.isPlaced = true;
        stackedIngredients.Add(ingredient);
        // 🔥 PRIMERO mover burger si es el primero
        if (stackedIngredients.Count == 1 && ingredient.type == IngredientType.BaseBread)
        {
            transform.position = ingredient.transform.position;
        }

        // 🔥 LUEGO hacer parent
        ingredient.transform.SetParent(this.transform);
        ingredient.transform.localPosition = new Vector3(0, currentHeight, 0);
        // 🔥 Pasar a Burger
        if (burger != null)
            burger.AddIngredient(ingredient);

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = true;
        }

        ingredient.transform.SetParent(this.transform);
        ingredient.transform.localPosition = new Vector3(0, currentHeight, 0);
        ingredient.transform.localRotation = Quaternion.identity;

        float h = heightStep;
        Collider c = ingredient.GetComponent<Collider>();
        if (c != null) h = c.bounds.size.y;

        currentHeight += h;
        ignoreExitTimer = 0.25f;

        // 🔥 Cocción
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            finalCookState = meat.currentState;

            if (burger != null)
                burger.cookState = meat.currentState;
        }

        // 🔒 Cerrar burger
        if (ingredient.type == IngredientType.TopBread)
        {
            FinishBurger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ignoreExitTimer > 0) return;

        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient != null && ingredient.isPlaced && stackedIngredients.Contains(ingredient))
        {
            RemoveIngredient(ingredient);
        }
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        Debug.Log("➖ Quitando: " + ingredient.type);

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        ingredient.transform.SetParent(null);
        ingredient.isPlaced = false;
        stackedIngredients.Remove(ingredient);

        if (burger != null)
            burger.RemoveIngredient(ingredient);

        float h = heightStep;
        Collider c = ingredient.GetComponent<Collider>();
        if (c != null) h = c.bounds.size.y;

        currentHeight -= h;
    }

    private void FinishBurger()
    {
        Debug.Log("✅ Hamburguesa completada");

        foreach (Ingredient ing in stackedIngredients)
        {
            Interactable interactable = ing.GetComponent<Interactable>();
            if (interactable != null)
                interactable.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
            col.enabled = false;
    }
}