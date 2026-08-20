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

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashCooldown = 2.0f;
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;

    [Header("Agachado (Crouch)")]
    [SerializeField] private float multiplicadorAlturaCollider = 0.5f;
    private bool isCrouching = false;
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
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * speed * Time.deltaTime, 0, 0));

        if (isDashing) return;

        // --- LÓGICA DE AGACHARSE ---
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            Agacharse();
        }
        else if (Input.GetKeyUp(KeyCode.S) && isCrouching)
        {
            Levantarse();
        }

        // --- SALTO (No se permite si está agachado) --
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
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
    }

    private void Agacharse()
    {
        isCrouching = true;

        if (boxCollider != null)
        {
            // Reduce la altura del collider
            boxCollider.size = new Vector2(tamanoOriginalCollider.x, tamanoOriginalCollider.y * multiplicadorAlturaCollider);

            // Reajusta el offset para compensar la reducción desde el centro hacia la base
            float reduccion = tamanoOriginalCollider.y * (1f - multiplicadorAlturaCollider);
            boxCollider.offset = new Vector2(offsetOriginalCollider.x, offsetOriginalCollider.y - (reduccion / 2f));
        }
    }

    private void Levantarse()
    {
        isCrouching = false;

        if (boxCollider != null)
        {
            // Restaura las dimensiones y el offset original
            boxCollider.size = tamanoOriginalCollider;
            boxCollider.offset = offsetOriginalCollider;
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector2 posicionDestino = (Vector2)transform.position + new Vector2(dashDistance, 0f);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // FASE 1: Moverse hacia la derecha (Destino)
        while (Vector2.SqrMagnitude((Vector2) transform.position - posicionDestino ) > 0.05f)
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
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;

            // Si el personaje cae por una plataforma mientras estaba agachado, se restaura su collider
            if (isCrouching)
            {
                Levantarse();
            }
        }
    }
}