using UnityEngine;
using Valve.VR.InteractionSystem;
// IMPORTANTE: Si usas XR Interaction Toolkit de Unity, descomenta la siguiente línea:
// using UnityEngine.XR.Interaction.Toolkit;

public class BurgerBuilder : MonoBehaviour
{
    private Burger currentBurger;
    private float currentHeight = 0f;
    public float heightStep = 0.05f;
    private float ignoreExitTimer = 0f;

    public Transform spawnPoint;

    // Reducimos el temporizador cada frame para evitar fallos de colisión
    private void Update()
    {
        if (ignoreExitTimer > 0)
        {
            ignoreExitTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🚨 La mesa acaba de detectar un objeto llamado: " + other.gameObject.name);

        // 🔥 EL CAMBIO ESTÁ AQUÍ: Añadimos InParent
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        // Si no es un ingrediente o ya está colocado, lo ignoramos
        if (ingredient == null || ingredient.isPlaced) return;

        // Si no hay hamburguesa en proceso...
        if (currentBurger == null)
        {
            // Solo empezamos si el jugador pone el pan inferior
            if (ingredient.type == IngredientType.BaseBread)
            {
                StartNewBurger(ingredient);
            }
            return;
        }

        // Si ya hay una hamburguesa en proceso, le añadimos el ingrediente
        AddIngredient(ingredient);
    }

    private void OnTriggerExit(Collider other)
    {
        // Ignoramos si el objeto acaba de entrar
        if (ignoreExitTimer > 0) return;

        // 🔥 EL CAMBIO TAMBIÉN AQUÍ: Añadimos InParent
        Ingredient ingredient = other.GetComponentInParent<Ingredient>();

        // Si el jugador saca un ingrediente que ya estaba colocado
        if (ingredient != null && ingredient.isPlaced)
        {
            RemoveIngredient(ingredient);
        }
    }

    private void StartNewBurger(Ingredient baseBread)
    {
        Debug.Log("🍔 Hamburguesa iniciada");

        GameObject newBurgerObj = new GameObject("AssembledBurger");

        // 🔥 CAMBIO AQUÍ: Ahora usamos la posición de tu nuevo SpawnPoint
        newBurgerObj.transform.position = spawnPoint.position;
        newBurgerObj.transform.rotation = spawnPoint.rotation;

        currentBurger = newBurgerObj.AddComponent<Burger>();
        currentHeight = 0f;

        AddIngredient(baseBread);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (currentBurger == null) return;

        ingredient.isPlaced = true;
        currentBurger.AddIngredient(ingredient);

        Collider col = ingredient.GetComponent<Collider>();
        float height = heightStep;

        if (col != null)
        {
            height = col.bounds.size.y;
        }

        // ⚠️ Eliminamos el halfHeight porque el pivote de tu pan ya está en la base

        // 🔥 TRUCO PARA STEAMVR: Congelamos el objeto pero dejamos la gravedad encendida
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;  // Esto hace que se quede pegado a la mesa
            rb.useGravity = true;   // Esto hace que SteamVR recuerde que pesa si lo vuelves a agarrar
        }

        // NOTA: No desactivamos el componente Interactable aquí para que el jugador
        // pueda arrepentirse, agarrarlo de nuevo y quitarlo.

        ingredient.transform.SetParent(currentBurger.transform);

        // 🔥 Usamos solo currentHeight para apilar los ingredientes perfectamente
        ingredient.transform.localPosition = new Vector3(0, currentHeight, 0);
        ingredient.transform.localRotation = Quaternion.identity;

        currentHeight += height;

        // Prevenir que el trigger exit se dispare inmediatamente por culpa de las físicas
        ignoreExitTimer = 0.25f;

        // Actualizar estado de cocción de la carne
        temporaryCookableController meat = ingredient.GetComponent<temporaryCookableController>();
        if (meat != null)
        {
            currentBurger.cookState = meat.currentState;
        }

        // 🔥 Si ponemos el pan de arriba, cerramos la hamburguesa
        if (ingredient.type == IngredientType.TopBread)
        {
            FinishBurger();
        }
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        if (currentBurger == null) return;

        // 1. Restaurar físicas para que vuelva a caer si lo sueltan
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 2. Desvincularlo del padre (la hamburguesa)
        ingredient.transform.SetParent(null);
        ingredient.isPlaced = false;

        // 3. Quitarlo de la lista en el script Burger
        currentBurger.RemoveIngredient(ingredient);

        // 4. Restar la altura que ocupaba este ingrediente
        Collider col = ingredient.GetComponent<Collider>();
        float height = heightStep;
        if (col != null) height = col.bounds.size.y;
        currentHeight -= height;

        // 5. Si quitaron el pan inferior o ya no quedan ingredientes, cancelamos todo
        if (ingredient.type == IngredientType.BaseBread || currentBurger.ingredients.Count == 0)
        {
            CancelBurger();
        }
    }

    private void CancelBurger()
    {
        Debug.Log("🚫 Hamburguesa cancelada");

        if (currentBurger != null)
        {
            Destroy(currentBurger.gameObject);
            currentBurger = null;
        }
        currentHeight = 0f;
    }

    private void FinishBurger()
    {
        Debug.Log("✅ Hamburguesa terminada y lista para entregar");

        // 1. Ahora sí, desactivamos los componentes de agarre individuales
        foreach (Ingredient ing in currentBurger.ingredientObjects)
        {
            // Reemplaza "Interactable" por el nombre exacto de tu script de agarre (ej. XRGrabInteractable)
            Interactable interactable = ing.GetComponent<Interactable>();
            if (interactable != null) interactable.enabled = false;
        }

        // 2. Le damos físicas a la hamburguesa completa
        Rigidbody burgerRb = currentBurger.gameObject.AddComponent<Rigidbody>();
        burgerRb.useGravity = true;
        burgerRb.isKinematic = false;

        // 3. Le damos colisión e interacción VR al conjunto para agarrarla entera
        currentBurger.gameObject.AddComponent<BoxCollider>();
        // Reemplaza "Interactable" por tu script de VR
        currentBurger.gameObject.AddComponent<Interactable>();

        // 4. Vaciamos la tabla para el siguiente pedido
        currentBurger = null;
        currentHeight = 0f;
    }
}