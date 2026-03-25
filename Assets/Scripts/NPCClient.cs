using UnityEngine;
using UnityEngine.AI;

public class NPCClient : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agent;
    private Animator anim;
    private bool haLlegado = false;

    public float pacienciaMax = 30f;
    private float pacienciaActual;
    private bool esperando = false;
    private EstadoPaciencia estadoActual;


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
            pacienciaActual -= Time.deltaTime;
            EvaluarPaciencia();
        }
    }

    void Pedir()
    {
        Debug.Log("Quiero comida");
        esperando = true;
    }

    void Irse()
    {
        esperando = false;
        Destroy(gameObject);
    }

    void EvaluarPaciencia()
    {
        float porcentaje = pacienciaActual / pacienciaMax;

        if (porcentaje <= 0f)
        {
            CambiarEstado(EstadoPaciencia.Harto);
        }
        else if(porcentaje<=0.5f){
            CambiarEstado(EstadoPaciencia.Desesperado);
        }
        else{
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
                Debug.Log("Esperare");
                break;

            case EstadoPaciencia.Desesperado:
                Debug.Log("Me estoy desesperando");
                anim.speed = 2.0f;
                break;

            case EstadoPaciencia.Harto:
                Debug.Log("Estoy harto de esperar");
                Irse();
                break;
        }
    }

}