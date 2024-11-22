using UnityEngine;
using System.IO.Ports;

public class SkateController : MonoBehaviour
{
    // Variables para controlar el salto y estado del bot�n
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool buttonPressed = false;

    private Rigidbody rb;

    // Serial port para leer los datos de Arduino
    private SerialPort serialPort = new SerialPort("COM3", 9600);  // Cambia "COM3" por el puerto correcto

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        serialPort.Open();  // Abre el puerto serial
        serialPort.ReadTimeout = 1;  // Timeout peque�o para no bloquear
    }

    void Update()
    {
        // Verificar si el objeto est� en el suelo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);  // Verifica si el skate est� en el suelo

        // Leer datos del puerto serial (si est� disponible)
        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();  // Lee la l�nea del puerto serial
                if (data.Contains(",") && data.Split(',').Length >= 8)
                {
                    // Obtener el estado del bot�n (el valor es el �ltimo en la l�nea)
                    string buttonState = data.Split(',')[7].Trim();
                    buttonPressed = buttonState == "0";  // El bot�n est� presionado si el valor es 0 (normalmente, un bot�n con INPUT_PULLUP devuelve 0 cuando se presiona)
                }
            }
            catch (System.Exception)
            {
                // Si ocurre un error en la lectura, lo ignoramos
            }
        }

        // Si el bot�n est� presionado y el skate est� en el suelo, realiza el salto
        if (buttonPressed && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnApplicationQuit()
    {
        // Aseg�rate de cerrar el puerto serial al finalizar
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
