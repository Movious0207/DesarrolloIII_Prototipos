using System.Collections; // Necesario para usar Corrutinas (IEnumerator)
using UnityEngine;

public class KnifeControl : MonoBehaviour
{
    [Header("Configuración de Teclas")]
    public KeyCode teclaCorte = KeyCode.Space; // Tecla para picar

    [Header("Configuración del Movimiento")]
    public float distanciaDescenso = 2f; // Distancia que baja el cuchillo
    public float velocidadBajada = 15f;   // Rapidez al bajar para el corte
    public float velocidadSubida = 5f;    // Rapidez al regresar a la posición inicial

    [Header("Estado")]
    private bool estaObstaculizado = false;
    private GameObject verduraActual = null;
    private bool estaAnimando = false; // Evita que se corte múltiples veces a la vez

    private Vector3 posicionInicial;

    void Start()
    {
        // Guarda la posición inicial alta del cuchillo
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Solo permite iniciar un corte si presionas la tecla y no está ya bajando/subiendo
        if (Input.GetKeyDown(teclaCorte) && !estaAnimando)
        {
            StartCoroutine(AnimacionCorte());
        }
    }

    IEnumerator AnimacionCorte()
    {
        estaAnimando = true;
        Vector3 posicionDestino = posicionInicial + Vector3.down * distanciaDescenso;

        // 1. BAJAR: Movimiento rápido hacia abajo
        while (Vector3.Distance(transform.position, posicionDestino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionDestino, velocidadBajada * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }
        transform.position = posicionDestino;

        // 2. INTENTAR CORTAR: Evalúa el impacto justo en el punto más bajo
        IntentarCortar();

        // Breve pausa en el fondo para dar sensación de fuerza de impacto
        yield return new WaitForSeconds(0.05f);

        // 3. SUBIR: Regreso más suave a la posición inicial de descanso
        while (Vector3.Distance(transform.position, posicionInicial) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidadSubida * Time.deltaTime);
            yield return null;
        }
        transform.position = posicionInicial;

        estaAnimando = false;
    }

    void IntentarCortar()
    {
        if (estaObstaculizado)
        {
            Debug.Log("¡Corte bloqueado por un obstáculo!");
            return;
        }

        if (verduraActual != null)
        {
            Debug.Log("¡Verdura cortada!");
            Destroy(verduraActual); // Destruye la verdura (simula el corte)
            verduraActual = null;   // Limpia la referencia
        }
        else
        {
            Debug.Log("Cortaste al aire...");
        }
    }

    // Detecta qué entra en el área del cuchillo
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstaculo"))
        {
            estaObstaculizado = true;
        }
        else if (other.CompareTag("Verdura"))
        {
            verduraActual = other.gameObject;
        }
    }

    // Detecta qué sale del área del cuchillo
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstaculo"))
        {
            estaObstaculizado = false;
        }
        else if (other.CompareTag("Verdura") && other.gameObject == verduraActual)
        {
            verduraActual = null;
        }
    }
}
