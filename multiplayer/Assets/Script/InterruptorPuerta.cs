using UnityEngine;

public class InterruptorPuerta : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject puerta; // Arrastra la puerta aquí desde el Inspector

    [Header("Configuración Visual")]
    public Color colorPresionado = Color.green;
    public Color colorOriginal = Color.yellow;

    [Header("Ajustes de Movimiento de la Puerta")]
    public Vector2 desplazamientoPuerta = new Vector2(0, 4f); // Cuánto se moverá al abrirse (X, Y)
    public float velocidadPuerta = 5f;

    private SpriteRenderer spriteRenderer;
    private Vector2 posicionInicialPuerta;
    private Vector2 posicionObjetivoPuerta;
    private int jugadoresEncima = 0; // Contador por si ambos jugadores se paran encima a la vez

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (puerta != null)
        {
            posicionInicialPuerta = puerta.transform.position;
            posicionObjetivoPuerta = posicionInicialPuerta;
        }
    }

    void Update()
    {
        // Mueve la puerta suavemente hacia su posición actual objetivo (abierta o cerrada)
        if (puerta != null)
        {
            puerta.transform.position = Vector2.MoveTowards(
                puerta.transform.position,
                posicionObjetivoPuerta,
                velocidadPuerta * Time.deltaTime
            );
        }
    }

    // Se activa cuando un jugador pisa el interruptor
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que pisó el botón tiene el script de movimiento de jugador
        if (collision.GetComponent<MovimientoJugador>() != null)
        {
            jugadoresEncima++;

            if (jugadoresEncima == 1) // Si es el primer jugador en pisarlo, se activa
            {
                AbrirPuerta();
            }
        }
    }

    // Se activa cuando un jugador se baja del interruptor
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovimientoJugador>() != null)
        {
            jugadoresEncima--;

            if (jugadoresEncima <= 0) // Si ya no queda ningún jugador encima, se desactiva
            {
                jugadoresEncima = 0;
                CerrarPuerta();
            }
        }
    }

    void AbrirPuerta()
    {
        spriteRenderer.color = colorPresionado;
        posicionObjetivoPuerta = posicionInicialPuerta + desplazamientoPuerta;
    }

    void CerrarPuerta()
    {
        spriteRenderer.color = colorOriginal;
        posicionObjetivoPuerta = posicionInicialPuerta;
    }
}
