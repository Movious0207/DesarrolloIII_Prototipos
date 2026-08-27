using System.Collections;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded;

    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashCooldown = 2.0f;
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;

    [Header("Agachado (Crouch)")]
    [SerializeField] private float multiplicadorAlturaCollider = 0.5f;
    [SerializeField] private LayerMask techoLayer; // Asigna aquí la capa de los obstáculos/techos
    [SerializeField] private float margenPosterior = 1f; // Extensión extra hacia atrás
    [SerializeField] private float margenAncho = 1f; // Extensión extra de ancho general

    private bool isCrouching = false;
    private bool wantsToStandUp = false; // Controla si el jugador soltó la tecla S
    private BoxCollider2D boxCollider;
    private Vector2 tamanoOriginalCollider;
    private Vector2 offsetOriginalCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            tamanoOriginalCollider = boxCollider.size;
            offsetOriginalCollider = boxCollider.offset;
        }
        jumpsRemaining = maxJumps;
        currentSpeed = speed;
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));

        if (isDashing) return;

        // --- LÓGICA DE AGACHARSE Y LEVANTARSE ---
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            Agacharse();
        }
        else if (Input.GetKeyUp(KeyCode.S) && isCrouching)
        {
            wantsToStandUp = true;
        }

        // Intenta levantarse frame a frame si el jugador soltó S pero había un techo encima
        if (wantsToStandUp && isCrouching)
        {
            TratardeLevantarse();
        }

        // --- SALTO (No se permite si está agachado) ---
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0 && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--;
            isGrounded = false;
        }

        // --- DASH (No se permite si está agachado) ---
        if (Input.GetKeyDown(KeyCode.D) && !isCrouching && canDash)
        {
            StartCoroutine(DashRoutine());
        }
        if (!canDash)
        {
            dashTimer -= Time.deltaTime;
        }
        if (dashTimer < 0 && !canDash && isGrounded)
        {
            canDash = true;
        }

        if (currentSpeed < speed)
            currentSpeed += 0.1f;
    }

    private void Agacharse()
    {
        isCrouching = true;
        wantsToStandUp = false;

        if (boxCollider != null)
        {
            // Reduce la altura del collider
            boxCollider.size = new Vector2(tamanoOriginalCollider.x, tamanoOriginalCollider.y * multiplicadorAlturaCollider);

            // Reajusta el offset para compensar la reducción desde el centro hacia la base
            float reduccion = tamanoOriginalCollider.y * (1f - multiplicadorAlturaCollider);
            boxCollider.offset = new Vector2(offsetOriginalCollider.x, offsetOriginalCollider.y - (reduccion / 2f));
        }
    }

    private void TratardeLevantarse()
    {
        // Si no hay obstáculo arriba, se levanta por completo
        if (!HayTechoEncima())
        {
            isCrouching = false;
            wantsToStandUp = false;

            if (boxCollider != null)
            {
                // Restaura las dimensiones y el offset original
                boxCollider.size = tamanoOriginalCollider;
                boxCollider.offset = offsetOriginalCollider;
            }
        }
    }

    private bool HayTechoEncima()
    {
        if (boxCollider == null) return false;

        // Calculamos el centro de la caja de verificación al estar de pie
        Vector2 centroDePie = (Vector2)transform.position + offsetOriginalCollider;

        // Determinamos la dirección del movimiento (1 si va a la derecha, -1 si va a la izquierda)
        float direccion = Mathf.Sign(currentSpeed);

        // Desplazamos la caja hacia ATRÁS según la dirección en que avanza el jugador
        centroDePie.x -= (direccion * margenPosterior / 2f);

        // Definimos el tamaño del área de verificación ampliando los márgenes
        Vector2 tamanoDePie = new Vector2(tamanoOriginalCollider.x + margenPosterior + margenAncho, tamanoOriginalCollider.y);

        // Comprobamos la zona ampliada mediante OverlapBox
        Collider2D solape = Physics2D.OverlapBox(centroDePie, tamanoDePie, 0f, techoLayer);

        return solape != null;
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector2 posicionDestino = (Vector2)transform.position + new Vector2(dashDistance, 0f);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // FASE 1: Moverse hacia la derecha (Destino)
        while (Vector2.SqrMagnitude((Vector2)transform.position - posicionDestino) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, posicionDestino, dashSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = posicionDestino;
        rb.gravityScale = gravedadOriginal;
        isDashing = false;
        canDash = false;
        dashTimer = dashCooldown;
    }

    // --- DETECCIÓN DE SUELO ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            jumpsRemaining = maxJumps;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            currentSpeed *= -1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;

            // Si el personaje cae por una plataforma mientras estaba agachado y ya no hay techo
            if (isCrouching && !HayTechoEncima())
            {
                isCrouching = false;
                wantsToStandUp = false;

                if (boxCollider != null)
                {
                    boxCollider.size = tamanoOriginalCollider;
                    boxCollider.offset = offsetOriginalCollider;
                }
            }
        }
    }

    // Muestra la caja de comprobación física en el editor de Unity
    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        Vector2 centroDePie = (Vector2)transform.position + offsetOriginalCollider;
        float direccion = Application.isPlaying ? Mathf.Sign(currentSpeed) : 1f;

        centroDePie.x -= (direccion * margenPosterior / 2f);
        Vector2 tamanoDePie = new Vector2(tamanoOriginalCollider.x + margenPosterior + margenAncho, tamanoOriginalCollider.y);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(centroDePie, tamanoDePie);
    }
}