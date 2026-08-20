using System.Collections;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody2D rb;
    [SerializeField] private bool isGrounded;

    [SerializeField] private float dashDistance = 3f;     
    [SerializeField] private float dashSpeed = 8f;     
    [SerializeField] private float returnSpeed = 5f;
    private bool isDashing = false;

    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if (isDashing) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.D) && isGrounded && !isCrouching)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector2 posicionOriginal = transform.position;
        Vector2 posicionDestino = posicionOriginal + new Vector2(dashDistance, 0f);

        float gravedadOriginal = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // 2. FASE 1: Moverse hacia la derecha (Destino)
        while (Vector2.Distance(transform.position, posicionDestino) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, posicionDestino, dashSpeed * Time.deltaTime);
            yield return null; // Espera al siguiente frame
        }
        transform.position = posicionDestino; // Aseguramos posición exacta al llegar

        // Opcional: Puedes meter un minúsculo tiempo de espera aquí si quieres que se quede quieto un instante
        // yield return new WaitForSeconds(0.05f);

        // 3. FASE 2: Regresar a la posición original
        while (Vector2.Distance(transform.position, posicionOriginal) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, posicionOriginal, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = posicionOriginal; // Aseguramos posición exacta al regresar

        // 4. Restauramos el estado normal del personaje
        rb.gravityScale = gravedadOriginal;
        isDashing = false;
    }

    // --- DETECCIÓN DE SUELO ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
}