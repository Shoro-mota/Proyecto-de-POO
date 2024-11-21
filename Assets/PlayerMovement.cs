using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Para la comunicación serie

public class SkateboardMovement : MonoBehaviour
{
    // Movimiento
    public float playerSpeed;  // Velocidad de movimiento
    public float jumpForce;    // Fuerza del salto
    private bool isGrounded;   // Verifica si está en el suelo
    private Rigidbody2D rb;

    // Giroscopio
    SerialPort serialPort = new SerialPort("COM7", 9600);  // Asegúrate de que el puerto sea el correcto
    float rotationX = 0f;
    float rotationY = 0f;

    void Start()
    {
        // Inicializamos el Rigidbody
        rb = GetComponent<Rigidbody2D>();

        // Abrimos la conexión del puerto serie para el giroscopio
        serialPort.Open();
        serialPort.ReadTimeout = 100; // Tiempo de espera para la lectura
    }

    void Update()
    {
        // Movimiento hacia adelante
        MoveForward();

        // Saltar
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Leer y aplicar los datos del giroscopio para la rotación
        if (serialPort.IsOpen)
        {
            try
            {
                // Leemos una línea de datos desde el puerto serie
                string value = serialPort.ReadLine();
                print(value);  // Mostrar para depuración

                // Dividir los datos por coma
                string[] data = value.Split(',');

                // Verificar que haya 6 valores: aceleración y giroscopio
                if (data.Length == 6)
                {
                    // Convertir los datos de la cadena a float
                    if (float.TryParse(data[3], out rotationX) && float.TryParse(data[4], out rotationY))
                    {
                        // Aplicamos la rotación a la tabla de skate
                        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error en la lectura del puerto serie: " + ex.Message);
            }
        }
    }

    void FixedUpdate()
    {
        // Mantenemos el movimiento hacia adelante a una velocidad constante
        rb.velocity = new Vector2(playerSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        // Reseteamos la velocidad vertical antes de aplicar el salto
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;  // Evitar que salte nuevamente hasta aterrizar
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el jugador toca el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnApplicationQuit()
    {
        // Cerramos el puerto serie cuando la aplicación se cierre
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
