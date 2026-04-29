using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;

public class BurgerController : MonoBehaviour
{
    public List<Ingredient> ingredients = new();
    public float pattyCookTime = 0f;

    private List<GameObject> ingredientObjects = new();
    private GameObject newIngredient;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>() != null)
        {
            ingredientObjects.Add(other.gameObject);
            ingredients.Add(other.gameObject.GetComponent<Ingredient>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ingredientObjects.Contains(other.gameObject))
        {
            ingredientObjects.Remove(other.gameObject);
            ingredients.Remove(other.gameObject.GetComponent<Ingredient>());
        }
    }

    public void FinaliseBurger()
    {
        Debug.Log("finalising burger...");
        foreach (GameObject ingredient in ingredientObjects)
        {
            // fix all ingredients in place
            ingredient.GetComponent<Rigidbody>().isKinematic = true;
            ingredient.GetComponent<Rigidbody>().useGravity = false;
            ingredient.GetComponent<Interactable>().enabled = false;
            ingredient.GetComponent<Throwable>().enabled = false;
            ingredient.tag = "Untagged";
            ingredient.transform.parent = this.transform;

            // set pattyCookTime to max cookTime of added patties
            if (ingredient.GetComponent<temporaryCookableController>() != null)
                if (ingredient.GetComponent<temporaryCookableController>().cookTime > pattyCookTime)
                    pattyCookTime = ingredient.GetComponent<temporaryCookableController>().cookTime;

            // remove trigger collider
            Destroy(this.GetComponent<BoxCollider>());
        }
    }
}
