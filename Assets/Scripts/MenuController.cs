using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [System.Serializable]
    public class DatosNivel
    {
        public string nombreHeader;
        [TextArea(2, 5)]
        public string descripcion;
        public string nombreEscena; 
    }

    [Header("Configuración de Niveles")]
    public DatosNivel[] niveles;
    private int nivelActual = 0;

    [Header("Componentes de la Interfaz")]
    public TextMeshProUGUI txtHeader;
    public TextMeshProUGUI txtDescripcion;

    [Header("Efectos de Sonido (UI)")]
    public AudioSource sonidoSiguiente;     
    public AudioSource sonidoAnterior;      
    public AudioSource sonidoPlay;         

    void Start()
    {
        ActualizarInterfaz();
    }

    public void SiguienteNivel()
    {
        if (sonidoSiguiente != null) sonidoSiguiente.Play();

        nivelActual++;
        if (nivelActual >= niveles.Length)
        {
            nivelActual = 0; 
        }
        ActualizarInterfaz();
    }

    public void AnteriorNivel()
    {
        if (sonidoAnterior != null) sonidoAnterior.Play();

        nivelActual--;
        if (nivelActual < 0)
        {
            nivelActual = niveles.Length - 1;
        }
        ActualizarInterfaz();
    }

    void ActualizarInterfaz()
    {
        if (niveles.Length > 0)
        {
            txtHeader.text = niveles[nivelActual].nombreHeader;
            txtDescripcion.text = niveles[nivelActual].descripcion;
        }
    }

    public void JugarNivel()
    {
        if (niveles.Length > 0)
        {
            if (sonidoPlay != null) sonidoPlay.Play();

            string escenaACargar = niveles[nivelActual].nombreEscena;
            if (!string.IsNullOrEmpty(escenaACargar))
            {
                SceneManager.LoadScene(escenaACargar);
            }
            else
            {
                Debug.LogWarning("No se ha asignado un nombre de escena para este nivel.");
            }
        }
    }
}