using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // El manejo de Inputs en Update() es correcto para no perder pulsaciones
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Nota: En Unity 2025/2026 se usa rb.linearVelocity en lugar del antiguo rb.velocity. ¡Bien ahí!
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Evaluamos el primer punto de contacto
            // Una normal.y cercana a 1 significa que el suelo está completamente plano abajo
            if (collision.contacts[0].normal.y > 0.7f)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
}