using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // No se necesita actualizar la dirección en el Update si siempre se mueve a la derecha
    }

    void FixedUpdate()
    {
        // Establece una velocidad constante hacia la derecha
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }
}



