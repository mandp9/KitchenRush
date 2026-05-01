using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCClient : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agent;
    private Animator anim;
    private bool haLlegado = false;
    private int miSlotUI = -1;
    [Header("Paciencia")]
    public float delayAntesDeEsperar = 8f;
    private float timerInicio = 0f;

    public float pacienciaMax = 180f;
    private float pacienciaActual;
    private bool esperando = false;
    private EstadoPaciencia estadoActual;

    [Header("Pedido")]
    public Order currentOrder;

    // 🔥 lo que recibe
    private BurgerController burgerRecibida = null;
    private bool friesRecibidas = false;

    [Header("Efectos")]
    public GameObject efectoHumo;

    enum EstadoPaciencia
    {
        Tranquilo,
        Desesperado,
        Harto
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        agent.SetDestination(destino.position);
        pacienciaActual = pacienciaMax;
    }

    void Update()
    {
        if (!haLlegado)
        {
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                haLlegado = true;
                anim.SetBool("isWalking", false);
                Pedir();
            }
        }

        if (esperando)
        {
            timerInicio += Time.deltaTime;

            if (timerInicio >= delayAntesDeEsperar)
            {
                pacienciaActual -= Time.deltaTime;
                pacienciaActual = Mathf.Max(pacienciaActual, 0f);
                EvaluarPaciencia();
            }
        }
    }

    void Pedir()
    {
        Debug.Log("Quiero comida");

        timerInicio = 0f;
        pacienciaActual = pacienciaMax;

        currentOrder = new Order();
        currentOrder.requiredCookState = CookState.Rare;

        currentOrder.requiredIngredients = new List<IngredientType>()
        {
            IngredientType.TopBread,
            IngredientType.Meat,
            IngredientType.Tomato
        };

        currentOrder.requiresFries = true;
        currentOrder.requiresDrink = false;

        Debug.Log("Pedido: Rare + Tomato + Fries");

        esperando = true;

        OrderUIManager ui = FindObjectOfType<OrderUIManager>();

        string texto =
        "Pedido\n" +
        "Carne: " + currentOrder.requiredCookState + "\n" +
        "Ingredientes:\n";

        foreach (var ing in currentOrder.requiredIngredients)
        {
            texto += "- " + ing + "\n";
        }

        if (currentOrder.requiresFries)
            texto += "Fries\n";

        miSlotUI = ui.AsignarPedido(texto);
    }

    public void RecibirBurger(BurgerController burger)
    {
        burgerRecibida = burger;
        burger.transform.parent = this.transform;

        Debug.Log("🍔 Burger recibida");

        ComprobarPedidoCompleto();
    }

    public void RecibirFries(GameObject fries)
    {
        friesRecibidas = true;

        Debug.Log("🍟 Fries recibidas");

        ComprobarPedidoCompleto();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!esperando) return;

        // 🍔 BURGER
        BurgerController burger = other.GetComponent<BurgerController>();
        if (burger != null && burger.ingredients.Count > 0)
        {
            RecibirBurger(burger);
            return;
        }

        // 🍟 FRIES (por nombre del objeto)
        if (other.gameObject.name.Contains("frieswithcontainer"))
        {
            RecibirFries(other.gameObject);
            return;
        }
    }

    void ComprobarPedidoCompleto()
    {
        if (burgerRecibida == null) return;
        if (currentOrder.requiresFries && !friesRecibidas) return;

        esperando = false;

        bool pedidoCorrecto = EsPedidoCorrecto(burgerRecibida);

        if (currentOrder.requiresFries && !friesRecibidas)
        {
            pedidoCorrecto = false;
        }

        if (pedidoCorrecto)
        {
            Debug.Log("✅ Pedido COMPLETO y CORRECTO");
        }
        else
        {
            Debug.Log("❌ Pedido COMPLETO pero INCORRECTO");
        }

        Invoke("Irse", 1);
    }

   bool EsPedidoCorrecto(BurgerController burger)
    {
        Debug.Log("========== DEBUG PEDIDO ==========");

        // 🍔 INGREDIENTES BURGER
        Debug.Log("🍔 Burger ingredientes: " + string.Join(", ", burger.ingredients));

        // 📦 INGREDIENTES PEDIDO
        Debug.Log("📦 Pedido ingredientes: " + string.Join(", ", currentOrder.requiredIngredients));

        // 🍖 COCCIÓN
        Debug.Log("🔥 Cook burger: " + burger.pattyCookState);
        Debug.Log("🔥 Cook pedido: " + currentOrder.requiredCookState);

        // 🔍 DETALLE DE CADA INGREDIENTE
        foreach (IngredientType ing in burger.ingredients)
        {
            if (currentOrder.requiredIngredients.Contains(ing))
            {
                Debug.Log("✔ Ingrediente correcto: " + ing);
            }
            else if (ing == IngredientType.BaseBread)
            {
                Debug.Log("➖ BaseBread (ignorado)");
            }
            else
            {
                Debug.Log("❌ Ingrediente EXTRA: " + ing);
            }
        }

        // 🔍 INGREDIENTES FALTANTES
        foreach (IngredientType ing in currentOrder.requiredIngredients)
        {
            if (!burger.ingredients.Contains(ing))
            {
                Debug.Log("❌ Falta ingrediente: " + ing);
            }
        }

        // ❌ CHECK COCCIÓN
        if (burger.pattyCookState != currentOrder.requiredCookState)
        {
            Debug.Log("❌ ERROR: Cocción incorrecta");
            return false;
        }

        // ❌ CHECK FALTANTES
        foreach (IngredientType ing in currentOrder.requiredIngredients)
        {
            if (!burger.ingredients.Contains(ing))
            {
                return false;
            }
        }

        // ❌ CHECK EXTRAS (ignorando BaseBread)
        foreach (IngredientType ing in burger.ingredients)
        {
            if (ing == IngredientType.BaseBread) continue;

            if (!currentOrder.requiredIngredients.Contains(ing))
            {
                return false;
            }
        }

        Debug.Log("✅ RESULTADO: BURGER CORRECTA");

        return true;
    }

    void Irse()
    {
        esperando = false;
        OrderUIManager ui = FindObjectOfType<OrderUIManager>();

        if (miSlotUI != -1)
            ui.LiberarPedido(miSlotUI);
            
        GameObject humo = Instantiate(efectoHumo, anim.transform.position, Quaternion.identity);
        Destroy(humo, 0.5f);

        Destroy(gameObject);
    }

    void EvaluarPaciencia()
    {
        if (!esperando) return;

        float porcentaje = pacienciaActual / pacienciaMax;

        if (pacienciaActual <= 0f)
        {
            CambiarEstado(EstadoPaciencia.Harto);
        }
        else if (porcentaje <= 0.25f)
        {
            CambiarEstado(EstadoPaciencia.Desesperado);
        }
        else
        {
            CambiarEstado(EstadoPaciencia.Tranquilo);
        }
    }

    void CambiarEstado(EstadoPaciencia nuevoEstado)
    {
        if (estadoActual == nuevoEstado) return;

        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case EstadoPaciencia.Tranquilo:
                Debug.Log("Im waiting");
                break;

            case EstadoPaciencia.Desesperado:
                Debug.Log("Getting mad...");
                anim.speed = 2.0f;
                break;

            case EstadoPaciencia.Harto:
                Debug.Log("Im fed up of waiting");
                Irse();
                break;
        }
    }
}