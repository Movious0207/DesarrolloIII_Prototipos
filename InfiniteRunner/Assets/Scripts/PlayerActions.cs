using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerActions : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded;

    [SerializeField] private int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Start")]
    [SerializeField] private GameObject startPoint;

    [Header("Finish")]
    [SerializeField] private GameObject endPoint;

    [Header("Speed")]
    [SerializeField] private int speed = 10;
    private float currentSpeed;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashCooldown = 2.0f;
    [SerializeField] private LayerMask dashObstacleLayer; // Capa de los obstáculos que bloquean el Dash
    [SerializeField] private float margenPared = 1f; // Distancia de seguridad para no quedar pegado dentro de la pared
    private bool canDash = true;
    private float dashTimer = 0f;
    private bool isDashing = false;

    [Header("Agachado (Crouch)")]
    [SerializeField] private float multiplicadorAlturaCollider = 0.5f;
    [SerializeField] private LayerMask techoLayer;
    [SerializeField] private float margenPosterior = 1f;
    [SerializeField] private float margenAncho = 1f;

    [Header("Key change timer")]
    [SerializeField] private float keyTimer = 3.0f;

    // --- NUEVOS ARRAYS FILAS QWERTY ---
    private readonly KeyCode[] filaSuperior = {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
        KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
    };

    private readonly KeyCode[] filaMedia = {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
    };

    private readonly KeyCode[] filaInferior = {
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V,
        KeyCode.B, KeyCode.N, KeyCode.M
    };

    private bool isCrouching = false;
    private bool wantsToStandUp = false;
    private BoxCollider2D boxCollider;
    private Vector2 tamanoOriginalCollider;
    private Vector2 offsetOriginalCollider;

    // Teclas iniciales asignadas según su respectiva fila
    private KeyCode jumpKey = KeyCode.W; // Cambiará a la fila superior al iniciar/restablecer
    private KeyCode dashKey = KeyCode.D;
    private KeyCode crouchKey = KeyCode.Z;

    private float currentKeyTimer = 0.0f;

    Vector3 posicionPantalla;

    void Start()
    {
        transform.position = startPoint.transform.position;

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            tamanoOriginalCollider = boxCollider.size;
            offsetOriginalCollider = boxCollider.offset;
        }

        jumpsRemaining = maxJumps;

        currentSpeed = speed;
        currentKeyTimer = keyTimer;
    }

    void Update()
    {
        transform.Translate(new Vector3(1 * currentSpeed * Time.deltaTime, 0, 0));

        if (isDashing) return;
        // --- LÓGICA DE AGACHARSE ---
        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            Agacharse();
        }
        else if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            wantsToStandUp = true;
        }

        if (wantsToStandUp && isCrouching)
        {
            TratardeLevantarse();
        }

        // --- SALTO (No se permite si está agachado) --
        if (Input.GetKeyDown(jumpKey) && jumpsRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpsRemaining--;
            isGrounded = false;
        }

        // --- DASH (No se permite si está agachado) ---
        if (Input.GetKeyDown(dashKey) && !isCrouching && canDash)
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

        if (currentKeyTimer <= 0.0f)
        {
            SetRandKey();

            currentKeyTimer += keyTimer;
        }

        if (currentSpeed < speed)
            currentSpeed += 0.1f;

        currentKeyTimer -= Time.deltaTime;

        posicionPantalla = Camera.main.WorldToViewportPoint(transform.position);

        if (posicionPantalla.y < 0)
        {
            transform.position = startPoint.transform.position;
        }
    }

    // Método modificado para elegir una tecla aleatoria de un array específico
    private KeyCode GetRandKeyFromRow(KeyCode[] fila)
    {
        int randomIndex = UnityEngine.Random.Range(0, fila.Length);
        return fila[randomIndex];
    }

    private void SetRandKey()
    {
        int randNum = UnityEngine.Random.Range(0, 3);

        switch (randNum)
        {
            case 0: // JUMP -> Fila Superior
                KeyCode prevJump = jumpKey;
                do
                {
                    jumpKey = GetRandKeyFromRow(filaSuperior);
                }
                // Evitamos que se repita la misma tecla de salto anterior
                while (jumpKey == prevJump);
                break;

            case 1: // DASH -> Fila Media
                KeyCode prevDash = dashKey;
                do
                {
                    dashKey = GetRandKeyFromRow(filaMedia);
                }
                while (dashKey == prevDash);
                break;

            case 2: // CROUCH -> Fila Inferior
                KeyCode prevCrouch = crouchKey;
                do
                {
                    crouchKey = GetRandKeyFromRow(filaInferior);
                }
                while (crouchKey == prevCrouch);
                break;
        }
    }

    private void Agacharse()
    {
        isCrouching = true;
        wantsToStandUp = false;

        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(tamanoOriginalCollider.x, tamanoOriginalCollider.y * multiplicadorAlturaCollider);
            float reduccion = tamanoOriginalCollider.y * (1f - multiplicadorAlturaCollider);
            boxCollider.offset = new Vector2(offsetOriginalCollider.x, offsetOriginalCollider.y - (reduccion / 2f));
        }
    }

    private void TratardeLevantarse()
    {
        if (!HayTechoEncima())
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

    private bool HayTechoEncima()
    {
        if (boxCollider == null) return false;

        Vector2 centroDePie = (Vector2)transform.position + offsetOriginalCollider;
        float direccion = Mathf.Sign(currentSpeed);

        centroDePie.x -= (direccion * margenPosterior / 2f);
        Vector2 tamanoDePie = new Vector2(tamanoOriginalCollider.x + margenPosterior + margenAncho, tamanoOriginalCollider.y);

        Collider2D solape = Physics2D.OverlapBox(centroDePie, tamanoDePie, 0f, techoLayer);

        return solape != null;
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        float direccionX = Mathf.Sign(currentSpeed);
        Vector2 direccionDash = new Vector2(direccionX, 0f);

        float distanciaEfectiva = dashDistance;

        Vector2 origenCast = (Vector2)transform.position + boxCollider.offset;
        Vector2 tamanoCaja = new Vector2(0.05f, boxCollider.size.y * 0.9f);

        RaycastHit2D hit = Physics2D.BoxCast(origenCast, tamanoCaja, 0f, direccionDash, dashDistance, dashObstacleLayer);

        if (hit.collider != null)
        {
            distanciaEfectiva = hit.distance - margenPared;
            if (distanciaEfectiva < 0) distanciaEfectiva = 0;
        }

        Vector2 posicionDestino = (Vector2)transform.position + new Vector2(direccionX * distanciaEfectiva, 0f);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.SqrMagnitude((Vector2)transform.position - posicionDestino) > 0.02f)
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

        if (collision.gameObject.CompareTag("Finish"))
        {
            Time.timeScale = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;

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

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        // Visualización del techo
        Vector2 centroDePie = (Vector2)transform.position + offsetOriginalCollider;
        float direccion = Application.isPlaying ? Mathf.Sign(currentSpeed) : 1f;

        centroDePie.x -= (direccion * margenPosterior / 2f);
        Vector2 tamanoDePie = new Vector2(tamanoOriginalCollider.x + margenPosterior + margenAncho, tamanoOriginalCollider.y);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(centroDePie, tamanoDePie);

        // Visualización del Dash Raycast
        Gizmos.color = Color.cyan;
        Vector2 origenDash = (Vector2)transform.position + boxCollider.offset;
        Gizmos.DrawRay(origenDash, new Vector2(direccion * dashDistance, 0f));
    }
#endif
    public KeyCode GetJumpKey()
    {
        return jumpKey;
    }

    public KeyCode GetDashKey()
    {
        return dashKey;
    }

    public KeyCode GetCrouchKey()
    {
        return crouchKey;
    }
}