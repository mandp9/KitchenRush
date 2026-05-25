using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;

public class BurgerController : MonoBehaviour
{
    public List<IngredientType> ingredients = new();
    public CookState pattyCookState;
    public AudioSource finaliseSound;

    private List<GameObject> ingredientObjects = new();
    private List<CookState> cookStates = new();
    private GameObject newIngredient;

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
        finaliseSound.Play();

        if (!ingredients.Contains(IngredientType.BaseBread))
        {
            ingredients.Add(IngredientType.BaseBread);
            Debug.Log("➕ BaseBread añadido manualmente");
        }

        foreach (GameObject ingredient in ingredientObjects)
        {
            DisableVRPhysics(ingredient);
            ingredient.transform.parent = this.transform;

            // check patty cook states; all patties must be cooked to the same state
            if (ingredient.GetComponent<temporaryCookableController>() != null)
                cookStates.Add(ingredient.GetComponent<temporaryCookableController>().GetCookState());
        }

        if (cookStates.FindAll(cookState => cookState == cookStates[0]).Count == cookStates.Count)
            pattyCookState = cookStates[0];

        Destroy(this.GetComponent<BoxCollider>());
    }

    // yoinked from ndahai tyvm
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
}