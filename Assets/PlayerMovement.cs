using UnityEngine;
using System.Collections; // Necesario para la corutina

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;  // Velocidad normal
    public float jumpForce;
    public float speedMultiplier = 2f;  // Factor de multiplicación para la velocidad
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

    // Método para multiplicar la velocidad por un tiempo limitado (1 segundo)
    private void BoostSpeed()
    {
        playerSpeed *= speedMultiplier;  // Aumenta la velocidad
        StartCoroutine(ResetSpeedAfterDelay(1f));  // Llama a la corutina para restablecer la velocidad después de 1 segundo
    }

    // Corutina que espera un segundo y luego restaura la velocidad original
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Espera 1 segundo
        playerSpeed /= speedMultiplier;  // Restaura la velocidad original
    }

    // Método que se activa al entrar en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpeedBoost"))  // Asegúrate de que el trigger tenga el tag "SpeedBoost"
        {
            BoostSpeed();  // Llama al método que aumenta la velocidad
        }
    }
}
