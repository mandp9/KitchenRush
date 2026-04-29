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
    private bool isFinished = false;

    private void Start()
    {
        burger = GetComponent<Burger>();
        currentHeight = 0f;
    }

    private void Update()
    {
        if (ignoreExitTimer > 0)
            ignoreExitTimer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFinished) return;
        if (stackedIngredients.Count == 0) return;

        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null || ingredient.isPlaced || ingredient.gameObject == this.gameObject)
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

        if (burger != null)
            burger.AddIngredient(ingredient);

        // 🔥 DESACTIVAR VR
        Interactable interactable = ingredient.GetComponent<Interactable>();
        if (interactable != null)
            interactable.enabled = false;

        // 🔥 FIJAR (SIN FÍSICA)
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 🔥 CALCULAR ALTURA
        float h = heightStep;
        Collider c = ingredient.GetComponent<Collider>();
        if (c != null) h = c.bounds.size.y;

        // 🔥 PARENT
        ingredient.transform.SetParent(this.transform, false);

        // 🔥 POSICIÓN CORRECTA
        if (stackedIngredients.Count == 1)
        {
            ingredient.transform.localPosition = new Vector3(0, h * 0.5f, 0);
        }
        else
        {
            ingredient.transform.localPosition = new Vector3(0, currentHeight + h * 0.5f, 0);
        }

        ingredient.transform.localRotation = Quaternion.identity;

        // 🔥 SUMAR ALTURA SOLO UNA VEZ
        currentHeight += h;
        ignoreExitTimer = 0.25f;

        // 🔥 COCCIÓN
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            finalCookState = meat.currentState;

            if (burger != null)
                burger.cookState = meat.currentState;
        }

        // 🔒 FINALIZAR
        if (ingredient.type == IngredientType.TopBread)
        {
            isFinished = true;
            FinishBurger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ignoreExitTimer > 0) return;
        if (isFinished) return;

        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient != null && ingredient.isPlaced && stackedIngredients.Contains(ingredient))
        {
            RemoveIngredient(ingredient);
        }
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        if (ingredient.type == IngredientType.BaseBread && stackedIngredients.Count == 1)
            return;

        Debug.Log("➖ Quitando: " + ingredient.type);

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Interactable interactable = ingredient.GetComponent<Interactable>();
        if (interactable != null)
            interactable.enabled = true;

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
            Rigidbody rb = ing.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Interactable interactable = ing.GetComponent<Interactable>();
            if (interactable != null)
                interactable.enabled = true;
        }

        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb == null)
            mainRb = gameObject.AddComponent<Rigidbody>();

        mainRb.isKinematic = false;

        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
            col.enabled = false;
    }
}