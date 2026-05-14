using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    [Header("Puntuaciones")]
    public int puntosSatisfecho = 30;
    public int puntosOk = 20;
    public int puntosIncorrecto = -10;
    public int puntosTimeout = -20;

    [Header("Pedido")]
    public Order currentOrder;

    private BurgerController burgerRecibida = null;
    private GameObject friesRecibidas = null;

    [Header("Efectos")]
    public GameObject efectoHumo;

    [Header("Movimiento")]
    public float delayAntesDeIr = 5f;

    [Header("UI Paciencia")]
    public Image barraPaciencia;

    [Header("Efecto Paciencia")]
    public float umbralParpadeo = 0.3f; 
    public float velocidadParpadeo = 5f;

    private float tiempoParpadeo = 0f;
    private bool haEmpezadoAMoverse = false;
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

        pacienciaActual = pacienciaMax;

        Invoke(nameof(EmpezarAMoverse), delayAntesDeIr);
    }

    void EmpezarAMoverse()
    {
        agent.SetDestination(destino.position);
        haEmpezadoAMoverse = true;
    }

    void PararHablar()
    {
        anim.SetBool("isOrdering", false);
    }

    void Update()
    {
        if (!haLlegado)
        {
            anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

            if (haEmpezadoAMoverse && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f)
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
        if (barraPaciencia != null)
        {
            float ratio = pacienciaActual / pacienciaMax;

            // 🔋 cantidad
            barraPaciencia.fillAmount = ratio;

            // 🎨 color verde → rojo
            Color colorBase = Color.Lerp(Color.red, Color.green, ratio);

            // ⚠️ PARPADEO
            if (ratio <= umbralParpadeo)
            {
                tiempoParpadeo += Time.deltaTime * velocidadParpadeo;

                float t = (Mathf.Sin(tiempoParpadeo) + 1f) * 0.5f;

                // mezcla con blanco para parpadeo
                barraPaciencia.color = Color.Lerp(colorBase, Color.white, t);
            }
            else
            {
                barraPaciencia.color = colorBase;
                tiempoParpadeo = 0f;
            }
        }
    }

    private Order GenerateOrder()
    {
        Order newOrder = new();
        int ingredientCount = 2;

        // choose patty doneness (rare or well done)
        newOrder.requiredCookState = (CookState) Random.Range(1, 3);

        // choose ingredient count (2 to 5, including top bun)
        ingredientCount = Random.Range(2, 6);
        newOrder.requiredIngredients = new List<IngredientType>()
        {
                IngredientType.TopBread,
                IngredientType.Meat
        };

        // choose other ingredients
        for (int i = 2; i < ingredientCount; i++)
            // rember to update range when adding new ingredients!
            newOrder.requiredIngredients.Add((IngredientType) Random.Range(0, 5));

        // choose extras
        newOrder.requiresFries = Random.Range(0, 2) == 1;
        //newOrder.requiresDrink = Random.Range(0, 1) == 1;
        newOrder.requiresDrink = false;

        return newOrder;
    }

    void Pedir()
    {
        anim.SetBool("isOrdering", true);
        Invoke(nameof(PararHablar), 2.5f);
        timerInicio = 0f;
        pacienciaActual = pacienciaMax;

        currentOrder = GenerateOrder();

        esperando = true;

        OrderUIManager ui = FindObjectOfType<OrderUIManager>();

        string texto =
        "Order\n" +
        "Patty: " + currentOrder.requiredCookState + "\n" +
        "Ingredients:\n";

        foreach (var ing in currentOrder.requiredIngredients)
        {
            // skip listing top bun
            if (ing == IngredientType.TopBread) continue;
            texto += "- " + ing + "\n";
        }

        if (currentOrder.requiresFries)
            texto += "Fries\n";

        miSlotUI = ui.AsignarPedido(texto);
    }

    public void RecibirBurger(BurgerController burger)
    {
        burgerRecibida = burger;

        Debug.Log("Burger recibida");

        ComprobarPedidoCompleto();
    }

    public void RecibirFries(GameObject fries)
    {
        friesRecibidas = fries;

        Debug.Log("Fries recibidas");

        ComprobarPedidoCompleto();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!esperando) return;

        BurgerController burger = other.GetComponent<BurgerController>();
        if (burger != null && burger.ingredients.Count > 0)
        {
            RecibirBurger(burger);
            return;
        }

        if (other.gameObject.name.Contains("frieswithcontainer"))
        {
            RecibirFries(other.gameObject);
            return;
        }
    }

    void ComprobarPedidoCompleto()
    {
        if (burgerRecibida == null) return;
        if (currentOrder.requiresFries && friesRecibidas == null) return;

        esperando = false;

        bool pedidoCorrecto = EsPedidoCorrecto(burgerRecibida);

        if (currentOrder.requiresFries && friesRecibidas == null)
        {
            pedidoCorrecto = false;
        }

        if (pedidoCorrecto)
        {
            // remove collider
            Destroy(this.gameObject.GetComponent<BoxCollider>());

            Debug.Log("✅ Pedido COMPLETO y CORRECTO");

            if (estadoActual == EstadoPaciencia.Tranquilo)
                ScoreController.instance.UpdateScore(puntosSatisfecho);
            else
                ScoreController.instance.UpdateScore(puntosOk);
        }
        else
        {
            // remove collider
            Destroy(this.gameObject.GetComponent<BoxCollider>());

            Debug.Log("❌ Pedido COMPLETO pero INCORRECTO");
            ScoreController.instance.UpdateScore(puntosIncorrecto);
        }

        StartCoroutine(Irse(pedidoCorrecto));
    }

   bool EsPedidoCorrecto(BurgerController burger)
    {
        Debug.Log("========== DEBUG PEDIDO ==========");

        Debug.Log("Burger ingredientes: " + string.Join(", ", burger.ingredients));

        Debug.Log("Pedido ingredientes: " + string.Join(", ", currentOrder.requiredIngredients));

        Debug.Log("Cook burger: " + burger.pattyCookState);
        Debug.Log("Cook pedido: " + currentOrder.requiredCookState);

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

        foreach (IngredientType ing in currentOrder.requiredIngredients)
        {
            if (!burger.ingredients.Contains(ing))
            {
                Debug.Log("❌ Falta ingrediente: " + ing);
            }
        }

        if (burger.pattyCookState != currentOrder.requiredCookState)
        {
            Debug.Log("❌ ERROR: Cocción incorrecta");
            return false;
        }

        foreach (IngredientType ing in currentOrder.requiredIngredients)
        {
            if (!burger.ingredients.Contains(ing))
            {
                return false;
            }
        }

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

    IEnumerator Irse(bool takeOrder)
    {
        esperando = false;
        anim.SetBool("isOrdering", false);

        yield return new WaitForSeconds(1f);

        OrderUIManager ui = FindObjectOfType<OrderUIManager>();
        if (miSlotUI != -1)
            ui.LiberarPedido(miSlotUI);
            
        GameObject humo = Instantiate(efectoHumo, anim.transform.position, Quaternion.identity);
        Destroy(humo, 0.5f);

        if (takeOrder)
        {
            if (burgerRecibida != null) Destroy(burgerRecibida.gameObject);
            if (friesRecibidas != null) Destroy(friesRecibidas);
        }

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
                ScoreController.instance.UpdateScore(puntosTimeout);
                StartCoroutine(Irse(false));
                break;
        }
    }
}