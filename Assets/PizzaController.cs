using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;

public enum PizzaCookState
{
    Raw,
    Cooked,
    Burnt
}

public class PizzaController : MonoBehaviour
{
    [Header("Configuración de la Masa")]
    public Transform pizzaCenter; // Objeto vacío en el centro de la masa
    public Vector3 cheeseTargetScale = new Vector3(39.7f, 230f, 20f); // Escala del queso

    [Header("Distribución Automática de Ingredientes")]
    public float pizzaRadius = 0.4f;
    public float heightOffset = 0.01f;

    [Header("Receta Objetivo")]
    public List<IngredientQuantity> requiredRecipe = new List<IngredientQuantity>();

    [Header("Estado Actual")]
    public List<IngredientType> currentIngredients = new();
    private List<GameObject> attachedObjects = new();

    [Header("Estado de Cocción")]
    public PizzaCookState cookState = PizzaCookState.Raw;



    // Contador de ingredientes pequeños para calcular el patrón de distribución
    private int smallToppingsCount = 0;

    [System.Serializable]
    public struct IngredientQuantity
    {
        public IngredientType type;
        public int count;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponentInParent<Ingredient>() ?? other.GetComponentInChildren<Ingredient>();

        if (ingredient != null && !ingredient.isPlaced)
        {
            SnapIngredient(ingredient);
        }
    }

    private void SnapIngredient(Ingredient ingredient)
    {
        ingredient.isPlaced = true;
        GameObject ingObject = ingredient.gameObject;

        DisableVRPhysics(ingObject);

        // Hacerlo hijo de la PIZZA COMPLETA
        ingObject.transform.parent = this.transform.parent;

        Quaternion targetRotation = pizzaCenter.rotation * Quaternion.Euler(ingredient.customRotationOffset);

        if (ingredient.type == IngredientType.Mozarella)
        {
            // El queso va directo al centro, escalado y con su rotación corregida
            ingObject.transform.position = pizzaCenter.position;
            ingObject.transform.rotation = targetRotation;
            ingObject.transform.localScale = cheeseTargetScale;
        }
        else
        {
            // Distribución matemática para ingredientes pequeños
            Vector3 targetLocalPosition = CalculateAutomaticToppingPosition(smallToppingsCount);
            smallToppingsCount++;

            Vector3 targetWorldPosition = pizzaCenter.TransformPoint(targetLocalPosition);

            ingObject.transform.position = targetWorldPosition;

            ingObject.transform.rotation = targetRotation;

            float randomAngle = Random.Range(0f, 360f);
            ingObject.transform.RotateAround(ingObject.transform.position, pizzaCenter.up, randomAngle);
        }

        currentIngredients.Add(ingredient.type);
        attachedObjects.Add(ingObject);

        CheckRecipe();
    }

    //  distribuir homogéneamente los ingredientes en un círculo.
    private Vector3 CalculateAutomaticToppingPosition(int index)
    {
        // Si es el primer ingrediente pequeño, ponerlo cerca del centro
        if (index == 0)
        {
            return new Vector3(0, heightOffset, 0);
        }

        float goldenAngle = 2.399963f;

        float angle = index * goldenAngle;


        float maxExpectedToppings = 25f;
        float normRadius = Mathf.Min(1f, Mathf.Sqrt(index / maxExpectedToppings));
        float currentRadius = normRadius * pizzaRadius;

        float x = Mathf.Cos(angle) * currentRadius;
        float z = Mathf.Sin(angle) * currentRadius;

        return new Vector3(x, heightOffset, z);
    }

    private void DisableVRPhysics(GameObject obj)
    {
        Throwable throwable = obj.GetComponent<Throwable>();
        if (throwable != null)
        {
            foreach (Hand hand in Object.FindObjectsOfType<Hand>())
            {
                if (hand.currentAttachedObject == obj)
                {
                    hand.DetachObject(obj);
                }
            }
            Destroy(throwable);
        }

        if (obj.GetComponent<Interactable>() != null) Destroy(obj.GetComponent<Interactable>());
        if (obj.GetComponent<Rigidbody>() != null) Destroy(obj.GetComponent<Rigidbody>());

        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        obj.tag = "Untagged";
    }

    public bool CheckRecipe()
    {
        foreach (var req in requiredRecipe)
        {
            int currentCount = currentIngredients.FindAll(x => x == req.type).Count;
            if (currentCount < req.count)
            {
                return false;
            }
        }

        Debug.Log("¡Pizza completada con éxito según la receta!");
        return true;
    }

    public void SetCookState(PizzaCookState newState)
    {
        cookState = newState;
        //UpdateVisuals();
        Debug.Log("La pizza ahora está: " + cookState);
    }
}