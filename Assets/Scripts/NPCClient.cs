using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NPCClient : MonoBehaviour
{
    [Header("Slots Barra")]
    public Transform[] puntosBarra;

    [Header("Bandejas")]
    public TrayDelivery[] bandejas;

    [Header("Puntos Cola")]
    public Transform[] puntosCola;

    [Header("Waypoint Inicial")]
    public Transform destino;

    private NavMeshAgent agent;
    private Animator anim;

    private bool haLlegado = false;
    private bool slotReservado = false;
    private bool haEmpezadoAMoverse = false;
    private bool yendoABarra = false;

    private int miSlotUI = -1;

    [HideInInspector]
    public int posicionCola;

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
    private Drink bebidaRecibida = null;
    
    // --- NUEVO: Elemento recibido para Nivel 3 ---
    private GameObject kebabRecibido = null; 

    [Header("Efectos")]
    public GameObject efectoHumo;
    public AudioSource sonidoSatisfecho;
    public AudioSource sonidoOk;
    public AudioSource sonidoIncorrecto;

    [Header("Movimiento")]
    public float delayAntesDeIr = 5f;

    [Header("UI Paciencia")]
    public Image barraPaciencia;

    [Header("Efecto Paciencia")]
    public float umbralParpadeo = 0.3f;
    public float velocidadParpadeo = 5f;

    private float tiempoParpadeo = 0f;

    [Header("Level Configuration")]
    public bool isLevel3 = false; 

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

        if (barraPaciencia != null)
        {
            barraPaciencia.transform.parent.gameObject.SetActive(false);
        }

        agent.isStopped = true;

        Invoke(nameof(EmpezarAMoverse), delayAntesDeIr);
    }

    void EmpezarAMoverse()
    {
        haEmpezadoAMoverse = true;
        QueueManager.instance.EntrarCola(this);
        ActualizarPosicionCola();
    }

    public void ActualizarPosicionCola()
    {
        if (slotReservado)
            return;

        haLlegado = false;

        if (posicionCola >= 0 && posicionCola < puntosCola.Length)
        {
            agent.isStopped = false;
            anim.SetBool("isWalking", true);
            agent.SetDestination(puntosCola[posicionCola].position);
        }
    }

    void PararHablar()
    {
        anim.SetBool("isOrdering", false);
    }

    void Update()
    {
        anim.SetBool("isWalking", agent.velocity.magnitude > 0.1f);

        if (!haEmpezadoAMoverse)
            return;

        // SOLO cuando va hacia barra
        if (yendoABarra &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.magnitude < 0.1f)
        {
            yendoABarra = false;
            haLlegado = true;
            agent.isStopped = true;
            anim.SetBool("isWalking", false);
            Pedir();
        }

        // paciencia
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

        // barra paciencia
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

        if (isLevel3)
        {
            newOrder.requiredKebab = true; 

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
        }
        else
        {
            newOrder.requiredKebab = false;
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
        }

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
        string texto = "Order\n";

        if (currentOrder.requiredKebab)
        {
            texto += "Kebab wrap\n";
        }

        texto += "Patty: " + currentOrder.requiredCookState + "\n" + "Ingredients:\n";

        foreach (var ing in currentOrder.requiredIngredients)
        {
            if (ing == IngredientType.TopBread) continue;
            texto += "- " + ing + "\n";
        }

        if (currentOrder.requiresFries)
            texto += "Fries\n";

        if (currentOrder.requiresDrink)
            texto += "Drink: " + currentOrder.requiredDrink + "\n";

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

    public void RecibirKebab(GameObject kebab)
    {
        kebabRecibido = kebab;
        Debug.Log("Kebab recibido");
        ComprobarPedidoCompleto();
    }

    void ComprobarPedidoCompleto()
    {
        if (!esperando) return;

        if (isLevel3)
        {
            if (kebabRecibido == null || burgerRecibida == null) return;
        }
        else
        {
            if (burgerRecibida == null) return;
        }

        if (currentOrder.requiresFries && friesRecibidas == null) return;
        if (currentOrder.requiresDrink && bebidaRecibida == null) return;

        esperando = false;

        // Validamos la estructura de la hamburguesa
        bool pedidoCorrecto = EsPedidoCorrecto(burgerRecibida);
       
        if (currentOrder.requiresDrink && bebidaRecibida != null)
            if (bebidaRecibida.type != currentOrder.requiredDrink)
                pedidoCorrecto = false;
        
        if (isLevel3 && kebabRecibido == null)
            pedidoCorrecto = false;
        
        if (pedidoCorrecto)
        {
            Destroy(GetComponent<BoxCollider>());

            if (estadoActual == EstadoPaciencia.Tranquilo)
            {
                // Tus AudioSources originales vuelven a sonar aquí
                if (sonidoSatisfecho != null) sonidoSatisfecho.Play();
                ScoreController.instance.UpdateScore(puntosSatisfecho);
            }
            else
            {
                if (sonidoOk != null) sonidoOk.Play();
                ScoreController.instance.UpdateScore(puntosOk);
            }
        }
        else
        {
            Destroy(GetComponent<BoxCollider>());
            if (sonidoIncorrecto != null) sonidoIncorrecto.Play();
            ScoreController.instance.UpdateScore(puntosIncorrecto);
        }

        StartCoroutine(Irse(pedidoCorrecto));
    }

    bool EsPedidoCorrecto(BurgerController burger)
    {
        var orderIngredients = new List<IngredientType>(currentOrder.requiredIngredients);

        if (burger.pattyCookState != currentOrder.requiredCookState)
            return false;

        foreach (IngredientType ingredient in burger.ingredients)
        {
            if (ingredient == IngredientType.BaseBread) continue;
            if (orderIngredients.Count == 0) return false;

            if (orderIngredients.Contains(ingredient))
                orderIngredients.Remove(ingredient);
            else 
                return false;
        }

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
            bandejas[miSlotUI].LimpiarBandeja();
            bandejas[miSlotUI].npcActual = null;
        }
        slotReservado = false;
        QueueManager.instance.ActualizarCola();
        
        GameObject humo = Instantiate(
            efectoHumo,
            anim.transform.position,
            Quaternion.identity
        );

        Destroy(humo, 0.5f);

        if (takeOrder)
        {
            if (kebabRecibido != null)
            {
                Valve.VR.InteractionSystem.Hand hand = kebabRecibido.GetComponentInParent<Valve.VR.InteractionSystem.Hand>();
                if (hand != null)
                {
                    hand.DetachObject(kebabRecibido);
                    hand.HoverLock(null);
                }
                kebabRecibido.SetActive(false); 
            }

            if (burgerRecibida != null)
                Destroy(burgerRecibida.gameObject);

            if (friesRecibidas != null)
                Destroy(friesRecibidas);
            
            if (bebidaRecibida != null)
                Destroy(bebidaRecibida.gameObject);

            yield return new WaitForFixedUpdate();

            if (kebabRecibido != null)
                Destroy(kebabRecibido);
        }

        if (barraPaciencia != null)
        {
            barraPaciencia.transform.parent.gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }

    IEnumerator ActualizarColaDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        QueueManager.instance.ActualizarCola();
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

    public void IntentarIrABarra()
    {
        if (slotReservado || yendoABarra)
            return;

        OrderUIManager ui = FindObjectOfType<OrderUIManager>();
        int slot = ui.ReservarSlot();

        if (slot == -1) return;

        miSlotUI = slot;
        bandejas[slot].LimpiarBandeja();
        bandejas[slot].npcActual = this;
        slotReservado = true;
        yendoABarra = true;

        QueueManager.instance.SalirCola(this);

        destino = puntosBarra[slot];
        agent.isStopped = false;
        agent.SetDestination(destino.position);
        anim.SetBool("isWalking", true);
    }
}