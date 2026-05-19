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

            // check patty cook states; all patties must be cooked to the same state
            if (ingredient.GetComponent<temporaryCookableController>() != null)
                cookStates.Add(ingredient.GetComponent<temporaryCookableController>().GetCookState());
        }

        // if all the patties are cooked to the same state, set the burger cook state to it
        // i.e. if all cook states are equal to the first cook state
        if (cookStates.FindAll(cookState => cookState == cookStates[0]).Count == cookStates.Count)
            pattyCookState = cookStates[0];

        // remove base bread's trigger collider
        Destroy(this.GetComponent<BoxCollider>());
    }
}