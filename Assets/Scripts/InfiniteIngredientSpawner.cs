using UnityEngine;

public class InfiniteIngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public Transform spawnPoint;

    private GameObject currentInstance;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        currentInstance = Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);

        InfiniteIngredient item = currentInstance.GetComponent<InfiniteIngredient>();
        if (item != null)
        {
            item.originSpawner = this;
        }
    }

    // 🔥 SOLO se llama cuando el jugador lo coge
    public void OnItemTaken()
    {
        Spawn();
    }
}