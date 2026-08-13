using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float forwardSpeed = 8f;
    public float sideSpeed = 8f;

    [Header("Salto")]
    public float jumpForce = 8f;

    private Rigidbody rb;
    private bool grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGround();

        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 velocity = rb.linearVelocity;

        velocity.z = forwardSpeed;
        velocity.x = horizontal * sideSpeed;

        rb.linearVelocity = velocity;

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGround()
    {
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            0.6f
        );
    }
}