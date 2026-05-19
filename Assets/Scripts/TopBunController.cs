using UnityEngine;
using System.Collections;

public class TopBunController : MonoBehaviour
{
    [SerializeField]
    private bool sittingOnIngredient = false;
    [SerializeField]
    private bool inBurgerTrigger = false;

    private GameObject bottomBun;
    private bool finalising = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Ingredient>() != null)
            sittingOnIngredient = true;

        if (sittingOnIngredient && inBurgerTrigger)            
            StartCoroutine(SendFinaliseBurger());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Ingredient>().type == IngredientType.BaseBread)
        {
            inBurgerTrigger = true;
            bottomBun = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        sittingOnIngredient = false;
        inBurgerTrigger = false;
        bottomBun = null;
    }

    private IEnumerator SendFinaliseBurger()
    {
        yield return new WaitForSeconds(0.5f);
        if (!finalising)
        {
            finalising = true;
            bottomBun.GetComponent<BurgerController>().FinaliseBurger();
        }
    }
}
