using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Arduino arduino;  // Referencia al script que lee los datos del giroscopio

    public float playerSpeed;  // Velocidad normal
    public float jumpForce; // Fuerza de salto
    public float speedMultiplier = 2f;  // Factor de multiplicacin para la velocidad de los trigger
    private bool isGrounded; // saber si el jugador está en el suelo
    private Rigidbody2D rb; // Referencia al componente Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded || arduino.boton && isGrounded)
        {
            Jump();
        }
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void BoostSpeed()
    {
        playerSpeed *= speedMultiplier;  // Aumenta la velocidad
        StartCoroutine(ResetSpeedAfterDelay(1f));  // Llama a la corutina para restablecer la velocidad despu s de 1 segundo
    }

    // Corutina que espera un segundo y luego restaura la velocidad original
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Espera 1 segundo
        playerSpeed /= speedMultiplier;  // Restaura la velocidad original
    }

    // M todo que se activa al entrar en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpeedBoost"))  // Asegurar de que el trigger tenga el tag "SpeedBoost"
        {
            BoostSpeed();  // Llama al m todo que aumenta la velocidad
        }
    }
}
