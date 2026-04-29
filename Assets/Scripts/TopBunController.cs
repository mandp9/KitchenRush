using UnityEngine;

public class TopBunController : MonoBehaviour
{
    [SerializeField]
    private bool sittingOnIngredient = false;
    [SerializeField]
    private bool inBurgerTrigger = false;

    private GameObject bottomBun;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Ingredient>() != null)
            sittingOnIngredient = true;

        if (sittingOnIngredient && inBurgerTrigger)            
            Invoke("SendFinaliseBurger", 0.5f);
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

    private void SendFinaliseBurger()
    {
        bottomBun.GetComponent<BurgerController>().FinaliseBurger();
    }
}
