using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Arduino arduino;  // referencia al script que lee los datos del giroscopio

    public float playerSpeed;  // velocidad normal
    public float jumpForce; // fuerza de salto
    public float speedMultiplier = 2f;  // factor de multiplicacin para la velocidad de los trigger (turbonitro)
    private bool isGrounded; // saber si el jugador esta en el suelo
    private Rigidbody2D rb; // referencia al componente Rigidbody2D

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded || arduino.boton && isGrounded) //salto con boton arduino y tecla space
        {
            Jump();
        }

    }

    void FixedUpdate() // mejora la fisica del rigidbody2d
    {
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }

    private void Jump() //metodo de salto
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) //colision con el suelo
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void BoostSpeed() // Aumenta la velocidad turbonitro
    {
        playerSpeed *= speedMultiplier;  
        StartCoroutine(ResetSpeedAfterDelay(1f));  // llama a la corutina para restablecer la velocidad despues de 1 segundo
    }

    // Corutina que espera un segundo y luego restaura la velocidad original
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Espera 1 segundo
        playerSpeed /= speedMultiplier;  // restaura la velocidad original
    }

    // metodo que se activa al entrar en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SpeedBoost"))  // asegura de que se active el trigger cuando tenga el tag "SpeedBoost"
        {
            BoostSpeed();  // Llama al metodo que aumenta la velocidad turbonitro
        }
    }
}
