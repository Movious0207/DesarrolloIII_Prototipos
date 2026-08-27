using UnityEngine;

public class InterruptorPlataforma : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject plataforma; // Arrastra la plataforma aquí en el Inspector

    [Header("Configuración Visual")]
    public Color colorPresionado = Color.green;

    [Header("Movimiento Horizontal")]
    public float distanciaDerecha = 5f; // Cuántas unidades se moverá hacia la derecha
    public float velocidadPlataforma = 3f;

    private SpriteRenderer spriteRenderer;
    private Vector2 posicionInicial;
    private Vector2 posicionDestino;
    private bool yaActivado = false; // Evita que se repita la lógica

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (plataforma != null)
        {
            posicionInicial = plataforma.transform.position;
            // Calculamos el destino sumando la distancia en el eje X
            posicionDestino = new Vector2(posicionInicial.x + distanciaDerecha, posicionInicial.y);
        }
    }

    void Update()
    {
        // Si el interruptor ya fue activado, movemos la plataforma hacia la derecha de forma constante
        if (yaActivado && plataforma != null)
        {
            plataforma.transform.position = Vector2.MoveTowards(
                plataforma.transform.position,
                posicionDestino,
                velocidadPlataforma * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si un jugador pisa el botón y aún no ha sido activado
        if (!yaActivado && collision.GetComponent<MovimientoJugador>() != null)
        {
            yaActivado = true;
            spriteRenderer.color = colorPresionado; // Cambia a verde permanente
        }
    }
}
