using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCClient : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agent;
    private Animator anim;
    private bool haLlegado = false;

    [Header("Paciencia")]
    public float delayAntesDeEsperar = 8f;
    private float timerInicio = 0f;

    public float pacienciaMax = 180f;
    private float pacienciaActual;
    private bool esperando = false;
    private EstadoPaciencia estadoActual;

    [Header("Pedido")]
    public Order currentOrder;

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
            if (agent.velocity.magnitude > 0.1f)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }

            if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
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

                if (pacienciaActual < 0f)
                    pacienciaActual = 0f;

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
            // bottom bun implied
            IngredientType.TopBread,
            IngredientType.Meat,
            IngredientType.Tomato
        };

        Debug.Log("Pedido: Rare + Tomato");

        esperando = true;
    }

    public void RecibirBurger(BurgerController burger)
    {
        // give burger to npc
        burger.gameObject.transform.parent = this.transform;

        if (EsPedidoCorrecto(burger))
        {
            Debug.Log("Pedido correcto");
            Invoke("Irse", 1);
        }
        else
        {
            Debug.Log("Pedido incorrecto");
            Invoke("Irse", 1);
        }
    }

    bool EsPedidoCorrecto(BurgerController burger)
    {
        if (burger.pattyCookState == null) return false;

        if (burger.pattyCookState != currentOrder.requiredCookState)
            return false;

        foreach (IngredientType ing in currentOrder.requiredIngredients)
        {
            if (!burger.ingredients.Contains(ing))
                return false;
        }

        if (burger.ingredients.Count != currentOrder.requiredIngredients.Count)
            return false;

        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!esperando) return;

        BurgerController burger = other.GetComponent<BurgerController>();

        if (burger != null && burger.ingredients.Count > 0)
        {
            RecibirBurger(burger);
        }
    }

    void Irse()
    {
        esperando = false;
        GameObject humo = Instantiate(efectoHumo, anim.transform.position, Quaternion.identity);
        Destroy(humo, 0.5f);
        Destroy(gameObject);
    }

    void EvaluarPaciencia()
    {
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