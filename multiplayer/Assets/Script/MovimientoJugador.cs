using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;

    [Header("Controles Personalizados")]
    public string ejeHorizontal = "Horizontal";
    public KeyCode teclaSalto = KeyCode.Space;

    private Rigidbody2D rb;
    private bool mirandoDerecha = true;

    // Variables nuevas para el doble salto
    private int saltosRealizados = 0;
    private int saltosMaximos = 2; // Cambia a 3 si quieres triple salto en el futuro
    private bool enElSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. Movimiento Horizontal
        float movimientoX = Input.GetAxisRaw(ejeHorizontal);
        rb.velocity = new Vector2(movimientoX * velocidad, rb.velocity.y);

        // 2. Girar el personaje según la dirección
        if (movimientoX > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (movimientoX < 0 && mirandoDerecha)
        {
            Girar();
        }

        // 3. Detectar si está en el suelo (velocidad vertical casi cero)
        enElSuelo = Mathf.Abs(rb.velocity.y) < 0.01f;

        // Si está en el suelo, reiniciamos el contador de saltos
        if (enElSuelo)
        {
            saltosRealizados = 0;
        }

        // 4. Lógica de Salto y Doble Salto
        if (Input.GetKeyDown(teclaSalto))
        {
            // Puede saltar si está en el suelo O si aún le quedan saltos por usar en el aire
            if (enElSuelo || saltosRealizados < saltosMaximos)
            {
                rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
                saltosRealizados++; // Sumamos un salto al contador
            }
        }
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
