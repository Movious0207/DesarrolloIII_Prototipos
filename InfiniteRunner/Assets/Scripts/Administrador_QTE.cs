using System;
using TMPro;
using UnityEngine;

public class Administrador_QTE : MonoBehaviour
{
    [Header("Configuración de Tiempos")]
    [SerializeField] private float tiempoEntreQTE = 20f;
    [SerializeField] private float tiempoParaResponder = 2.5f;
    [SerializeField] private GameObject startPoint;

    [Header("Referencias UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI textoFlecha;

    private float cronometroNormal;
    private float cronometroRespuesta;
    private bool qteActivo = false;

    private KeyCode teclaCorrecta;
    private Vector2 posicionGuardadaPlayer;

    // Guardamos las referencias de los componentes del jugador por fuera
    private PlayerActions playerScript;
    private Rigidbody2D playerRb;

    // Flechas direccionales para el QTE
    private KeyCode[] flechas = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
    private string[] textosFlechas = { "PRESIONÁ: ↑ (ARRIBA)", "PRESIONÁ: ↓ (ABAJO)", "PRESIONÁ: ← (IZQUIERDA)", "PRESIONÁ: → (DERECHA)" };

    void Start()
    {
        // Buscamos al jugador y sus componentes en la escena automáticamente
        playerScript = FindFirstObjectByType<PlayerActions>();
        if (playerScript != null)
        {
            playerRb = playerScript.GetComponent<Rigidbody2D>();
        }

        cronometroNormal = tiempoEntreQTE;

        if (textoFlecha != null)
            textoFlecha.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!qteActivo)
        {
            ManejarModoNormal();
        }
        else
        {
            // Forzamos el congelamiento absoluto desactivando el script del jugador y frenando su física
            if (playerScript != null) playerScript.enabled = false;
            if (playerRb != null) playerRb.linearVelocity = Vector2.zero;

            ManejarModoQTE();
        }
    }

    void ManejarModoNormal()
    {
        cronometroNormal -= Time.deltaTime;

        if (cronometroNormal <= 0)
            ActivarQTE();
    }

    void ActivarQTE()
    {
        if (playerScript == null) return;

        qteActivo = true;
        cronometroRespuesta = tiempoParaResponder;

        // Guardamos la posición exacta antes del QTE para el reinicio
        posicionGuardadaPlayer = playerScript.transform.position;

        // Elegir flecha al azar
        int indiceAleatorio = UnityEngine.Random.Range(0, flechas.Length);
        teclaCorrecta = flechas[indiceAleatorio];

        // Mostrar texto
        if (textoFlecha != null)
        {
            textoFlecha.text = textosFlechas[indiceAleatorio];
            textoFlecha.color = Color.yellow;
            textoFlecha.gameObject.SetActive(true);
        }
    }

    void ManejarModoQTE()
    {
        cronometroRespuesta -= Time.deltaTime;

        if (cronometroRespuesta <= 0)
        {
            TerminarConFallo();
            return;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(teclaCorrecta))
                TerminarConExito();
            else if (PresionoFlechaIncorrecta())
                TerminarConFallo();
        }
    }

    bool PresionoFlechaIncorrecta()
    {
        foreach (KeyCode flecha in flechas)
        {
            if (Input.GetKeyDown(flecha) && flecha != teclaCorrecta)
                return true;
        }
        return false;
    }

    void TerminarConExito()
    {
        qteActivo = false;
        cronometroNormal = tiempoEntreQTE;

        if (textoFlecha != null)
            textoFlecha.gameObject.SetActive(false);

        // Devolvemos el control al script del jugador de forma segura
        if (playerScript != null) playerScript.enabled = true;

        UnityEngine.Debug.Log("¡QTE Correcto! Continuando juego.");
    }

    void TerminarConFallo()
    {
        qteActivo = false;
        cronometroNormal = tiempoEntreQTE;

        if (textoFlecha != null)
            textoFlecha.gameObject.SetActive(false);

        UnityEngine.Debug.Log("¡QTE Fallado! El jugador muere y reinicia en el lugar.");

        // Devolvemos el control, lo teletransportamos a donde empezó el QTE y frenamos su inercia
        if (playerScript != null)
        {
            playerScript.transform.position = startPoint.transform.position;
            playerScript.enabled = true;
        }
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }
}
