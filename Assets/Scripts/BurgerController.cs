using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;

public class BurgerController : MonoBehaviour
{
    public List<IngredientType> ingredients = new();
    public CookState? pattyCookState = null;

    private List<GameObject> ingredientObjects = new();
    private GameObject newIngredient;
    private float pattyCookTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>() != null)
        {
            ingredientObjects.Add(other.gameObject);
            ingredients.Add(other.gameObject.GetComponent<Ingredient>().type);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ingredientObjects.Contains(other.gameObject))
        {
            ingredientObjects.Remove(other.gameObject);
            ingredients.Remove(other.gameObject.GetComponent<Ingredient>().type);
        }
    }

   public void FinaliseBurger()
    {
        Debug.Log("finalising burger...");

        // 🔥 FORZAR BASE BREAD
        if (!ingredients.Contains(IngredientType.BaseBread))
        {
            ingredients.Add(IngredientType.BaseBread);
            Debug.Log("➕ BaseBread añadido manualmente");
        }

        foreach (GameObject ingredient in ingredientObjects)
        {
            // fix all ingredients in place
            Destroy(ingredient.GetComponent<Throwable>());
            Destroy(ingredient.GetComponent<Rigidbody>());
            Destroy(ingredient.GetComponent<Interactable>());
            ingredient.tag = "Untagged";
            ingredient.transform.parent = this.transform;

            if (ingredient.GetComponent<temporaryCookableController>() != null)
                if (ingredient.GetComponent<temporaryCookableController>().cookTime > pattyCookTime)
                {
                    pattyCookTime = ingredient.GetComponent<temporaryCookableController>().cookTime;
                    pattyCookState = ingredient.GetComponent<temporaryCookableController>().GetCookState();
                }

            Destroy(this.GetComponent<BoxCollider>());
        }
    }
}