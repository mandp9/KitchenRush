using UnityEngine;

public class InfiniteChipsSpawner : MonoBehaviour
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
        currentInstance = Instantiate(
            ingredientPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint.parent
        );

        InfiniteIngredient item =
            currentInstance.GetComponent<InfiniteIngredient>();

        if (item != null)
        {
            item.chipsSpawner = this;
        }
    }

    public void OnItemTaken()
    {
        Spawn();
    }
}