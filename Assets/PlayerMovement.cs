using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public float jumpForce;
    private bool isGrounded;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Detecta si el jugador quiere saltar y si está en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Establece una velocidad constante hacia la derecha
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        // Aplica una fuerza vertical para el salto
        rb.velocity = new Vector2(rb.velocity.x, 0); // Resetea la velocidad vertical antes de aplicar la fuerza
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false; // Asegura que no se pueda saltar nuevamente hasta que aterrice
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el jugador está tocando el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}



