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

    void Start()
    {
        ActualizarInterfaz();
    }

    // Cambiar al nivel siguiente
    public void SiguienteNivel()
    {
        nivelActual++;
        if (nivelActual >= niveles.Length)
        {
            nivelActual = 0; // Regresa al primer nivel si llega al final
        }
        ActualizarInterfaz();
    }

    // Cambiar al nivel anterior
    public void AnteriorNivel()
    {
        nivelActual--;
        if (nivelActual < 0)
        {
            nivelActual = niveles.Length - 1;
        }
        ActualizarInterfaz();
    }

    // Actualiza los textos en la pantalla
    void ActualizarInterfaz()
    {
        if (niveles.Length > 0)
        {
            txtHeader.text = niveles[nivelActual].nombreHeader;
            txtDescripcion.text = niveles[nivelActual].descripcion;
        }
    }

    // Cargar la escena seleccionada
    public void JugarNivel()
    {
        if (niveles.Length > 0)
        {
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