using UnityEngine;
using UnityEngine.AI;

public class NPCClient : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agent;
    private Animator anim;
    private bool haLlegado = false;

    public float pacienciaMax = 10f;
    private float pacienciaActual;
    private bool esperando = false;

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

            if(pacienciaActual <= 0f)
            {
                Irse();
            }
        }
    }

    void Pedir()
    {
        Debug.Log("Quiero comida");
        esperando = true;
    }

    void Irse()
    {
        Debug.Log("Me canse de esperar");
        esperando = false;
        Destroy(gameObject);
    }
  
}