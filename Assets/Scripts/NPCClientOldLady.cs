using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCClientOldLady : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform destino;
    public Transform puntoIntermedio;

    [Header("Bandejas")]
    public TrayDelivery[] bandejas;

    private NavMeshAgent agent;
    private Animator anim;

    private int miSlotUI = -1;

    [Header("Paciencia")]
    public float delayAntesDeEsperar = 8f;
    private float timerInicio = 0f;

    public float pacienciaMax = 90f;
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
    private Drink bebidaRecibida = null;

    [Header("Efectos")]
    public GameObject efectoHumo;

    [Header("Delay antes de empezar a caminar")]
    public float delayAntesDeIr = 5f;

    [Header("UI Paciencia")]
    public Image barraPaciencia;

    [Header("Efecto Paciencia")]
    public float umbralParpadeo = 0.3f;
    public float velocidadParpadeo = 5f;

    private float tiempoParpadeo = 0f;

    enum EstadoPaciencia
    {
        Tranquilo,
        Desesperado,
        Harto
    }

    enum EstadoMovimiento
    {
        EsperandoParaSalir,
        YendoAWaypoint,
        YendoABarra,
        EnBarra
    }

    private EstadoMovimiento estadoMovimiento;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        pacienciaActual = pacienciaMax;

        estadoMovimiento = EstadoMovimiento.EsperandoParaSalir;

        if (barraPaciencia != null)
        {
            barraPaciencia.transform.parent.gameObject.SetActive(false);
        }

        Invoke(nameof(EmpezarAMoverse), delayAntesDeIr);
    }

    void EmpezarAMoverse()
    {
        estadoMovimiento = EstadoMovimiento.YendoAWaypoint;

        agent.isStopped = false;

        anim.SetBool("isWalking", true);

        agent.SetDestination(puntoIntermedio.position);
    }

    void PararHablar()
    {
        anim.SetBool("isOrdering", false);
    }

    void Update()
    {
        if (estadoMovimiento == EstadoMovimiento.YendoAWaypoint &&
            !agent.pathPending &&
            agent.remainingDistance <= 0.15f)
        {
            estadoMovimiento = EstadoMovimiento.YendoABarra;

            agent.isStopped = false;

            anim.SetBool("isWalking", true);

            agent.SetDestination(destino.position);

            return;
        }

        if (estadoMovimiento == EstadoMovimiento.YendoABarra &&
            !agent.pathPending &&
            agent.remainingDistance <= 0.15f)
        {
            estadoMovimiento = EstadoMovimiento.EnBarra;

            agent.isStopped = true;

            anim.SetBool("isWalking", false);

            Pedir();
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

            barraPaciencia.fillAmount = ratio;

            Color colorBase = Color.Lerp(Color.red, Color.green, ratio);

            if (ratio <= umbralParpadeo)
            {
                tiempoParpadeo += Time.deltaTime * velocidadParpadeo;

                float t = (Mathf.Sin(tiempoParpadeo) + 1f) * 0.5f;

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

        int ingredientCount = Random.Range(2, 6);

        newOrder.requiredCookState = (CookState)Random.Range(1, 3);

        newOrder.requiredIngredients = new List<IngredientType>()
        {
            IngredientType.TopBread,
            IngredientType.Meat
        };

        for (int i = 2; i < ingredientCount; i++)
            newOrder.requiredIngredients.Add((IngredientType)Random.Range(0, 5));

        newOrder.requiredIngredients.Sort();

        newOrder.requiresFries = Random.value < 0.6f;
        newOrder.requiresDrink = Random.value < 0.7f;

        if (newOrder.requiresDrink)
            newOrder.requiredDrink = (DrinkType)Random.Range(0, 5);

        return newOrder;
    }

    void Pedir()
    {
        anim.SetBool("isOrdering", true);

        Invoke(nameof(PararHablar), 2.5f);

        if (barraPaciencia != null)
        {
            barraPaciencia.transform.parent.gameObject.SetActive(true);
        }

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
            if (ing == IngredientType.TopBread) continue;

            texto += "- " + ing + "\n";
        }

        if (currentOrder.requiresFries)
            texto += "Fries\n";

        if (currentOrder.requiresDrink)
        {
            texto += "Drink: " +
                    currentOrder.requiredDrink + "\n";
        }

        miSlotUI = ui.ReservarSlot();

        bandejas[miSlotUI].LimpiarBandeja();
        
        if (miSlotUI == -1)
        {
            Debug.Log("No hay hueco libre");

            esperando = false;

            StartCoroutine(Irse(false));

            return;
        }

        bandejas[miSlotUI].npcActual = this;

        ui.EscribirPedido(miSlotUI, texto);
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

    public void RecibirDrink(Drink drink)
    {
        bebidaRecibida = drink;

        Debug.Log("Drink recibida");

        ComprobarPedidoCompleto();
    }

    void ComprobarPedidoCompleto()
    {
        if (!esperando) return;

        if (burgerRecibida == null) return;
        if (currentOrder.requiresFries && friesRecibidas == null) return;
        if (currentOrder.requiresDrink && bebidaRecibida == null) return;

        esperando = false;

        bool pedidoCorrecto = EsPedidoCorrecto(burgerRecibida);

        if (currentOrder.requiresDrink)
            if (bebidaRecibida.type != currentOrder.requiredDrink)
                pedidoCorrecto = false;

        if (pedidoCorrecto)
        {
            Destroy(GetComponent<BoxCollider>());

            if (estadoActual == EstadoPaciencia.Tranquilo)
                ScoreController.instance.UpdateScore(puntosSatisfecho);
            else
                ScoreController.instance.UpdateScore(puntosOk);
        }
        else
        {
            Destroy(GetComponent<BoxCollider>());
            ScoreController.instance.UpdateScore(puntosIncorrecto);
        }

        StartCoroutine(Irse(pedidoCorrecto));
    }

    bool EsPedidoCorrecto(BurgerController burger)
    {
        var orderIngredients = currentOrder.requiredIngredients;

        if (burger.pattyCookState != currentOrder.requiredCookState)
            return false;

        // check burger ingredients
        foreach (IngredientType ingredient in burger.ingredients)
        {
            if (ingredient == IngredientType.BaseBread) continue;

            // if we're out of ingredients in the order, return false
            // (excess ingredients in burger)
            if (orderIngredients.Count == 0) return false;

            // if the ingredient was found in the order, remove it from the list
            if (orderIngredients.Contains(ingredient))
                orderIngredients.Remove(ingredient);

            // if the ingredient is not in the order, return false
            // (extraneous ingredient in burger)
            else return false;
        }

        // if after going through all the ingredients in the burger there's still
        // some left in the order, return false (missing ingredients in burger)
        if (orderIngredients.Count > 0) return false;

        return true;
    }

    IEnumerator Irse(bool takeOrder)
    {
        esperando = false;

        anim.SetBool("isOrdering", false);

        yield return new WaitForSeconds(1f);

        OrderUIManager ui = FindObjectOfType<OrderUIManager>();

        if (miSlotUI != -1)
        {
            ui.LiberarPedido(miSlotUI);

            yield return new WaitForSeconds(0.1f);

            bandejas[miSlotUI].LimpiarBandeja();

            bandejas[miSlotUI].npcActual = null;
        }

        yield return new WaitForEndOfFrame();

        QueueManager.instance.ActualizarCola();

        GameObject humo = Instantiate(
            efectoHumo,
            anim.transform.position,
            Quaternion.identity
        );

        if (barraPaciencia != null)
        {
            barraPaciencia.transform.parent.gameObject.SetActive(false);
        }

        Destroy(humo, 0.5f);

        if (takeOrder)
        {
            if (burgerRecibida != null)
                Destroy(burgerRecibida.gameObject);

            if (friesRecibidas != null)
                Destroy(friesRecibidas);

            if (bebidaRecibida != null)
                Destroy(bebidaRecibida.gameObject);
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

                esperando = false;

                ScoreController.instance.UpdateScore(puntosTimeout);

                StartCoroutine(Irse(false));
                break;
        }
    }
}