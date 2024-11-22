using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public float jumpForce;

    private bool isGrounded;
    private Rigidbody2D rb;

    [Header("Gyroscope Settings")]
    public PlayerGyro gyro; // Referencia al script PlayerGyro
    public float maxTiltAngle = 30f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        ApplyGyroTilt();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void ApplyGyroTilt()
    {
        if (gyro != null)
        {
            float tiltAngle = Mathf.Clamp(gyro.currentInclination, -maxTiltAngle, maxTiltAngle);
            transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
