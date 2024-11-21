using UnityEngine;
using System; // A�adido para usar TimeoutException
using System.IO.Ports; // Para la comunicaci�n serie

public class PlayerGyro : MonoBehaviour
{
    // Ajusta el puerto serie seg�n el que est�s usando
    SerialPort serialPort = new SerialPort("COM7", 9600);

    // Variable para almacenar la rotaci�n acumulada en el eje Z
    private Quaternion currentRotation = Quaternion.identity; // Comienza en identidad (sin rotaci�n)

    // Variable p�blica para ajustar la sensibilidad de la rotaci�n
    public float sensitivity = 1.0f; // Sensibilidad inicial (puedes cambiarla en el Inspector de Unity)

    // Variables para suavizar la lectura del giroscopio
    private float previousRotationY = 0f; // Variable para la rotaci�n anterior
    private float smoothedRotationY = 0f; // Variable para la rotaci�n suavizada
    public float smoothingFactor = 0.1f; // Factor de suavizado (puedes ajustarlo)

    // Umbral para ignorar peque�as fluctuaciones del giroscopio
    public float rotationThreshold = 0.01f; // Umbral m�nimo de rotaci�n para ser considerado

    // Tiempo de inactividad para resetear la rotaci�n (en segundos)
    public float resetTime = 1.0f; // Tiempo despu�s del cual se resetear� la rotaci�n si no se detectan movimientos significativos
    private float timeSinceLastMovement = 0f;

    // Variable para calibrar el giroscopio al inicio
    private float initialRotationY = 0f;

    void Start()
    {
        // Abrimos el puerto serie
        serialPort.Open();
        serialPort.ReadTimeout = 100; // Tiempo de espera en ms

        // Calibrar el giroscopio al principio para establecer un valor de referencia
        initialRotationY = 0f;
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                // Leer datos del puerto serie
                string value = serialPort.ReadLine();
                print(value); // Mostrar datos para depuraci�n

                // Dividir los datos en un array
                string[] data = value.Split(',');

                // Validar que la cadena contiene los 8 valores
                if (data.Length == 8)
                {
                    // Intentar convertir el valor del giro en el eje Y (�ndice 4 en los datos)
                    if (float.TryParse(data[4], out float rotationY))
                    {
                        // Si es la primera lectura, establecemos la referencia inicial
                        if (initialRotationY == 0f)
                        {
                            initialRotationY = rotationY;
                        }

                        // Restar la rotaci�n inicial para eliminar el drift
                        float deltaRotationY = rotationY - initialRotationY;

                        // Aplicar suavizado con un filtro de media m�vil
                        smoothedRotationY = Mathf.Lerp(previousRotationY, deltaRotationY, smoothingFactor);

                        // Usar el valor suavizado para la rotaci�n
                        float adjustedRotationY = smoothedRotationY * sensitivity;

                        // Aplicar un umbral para evitar rotaciones peque�as no deseadas
                        if (Mathf.Abs(adjustedRotationY) > rotationThreshold)
                        {
                            // Crear una rotaci�n incremental, pero ahora la diferencia ser� negativa para invertir el giro
                            Quaternion rotationDelta = Quaternion.Euler(0, 0, -adjustedRotationY); // Resta para invertir la rotaci�n

                            // Actualizar la rotaci�n acumulada multiplicando por la nueva rotaci�n
                            currentRotation *= rotationDelta;

                            // Resetear el tiempo desde el �ltimo movimiento significativo
                            timeSinceLastMovement = 0f;
                        }
                        else
                        {
                            // Si no hay movimiento significativo, incrementar el tiempo de inactividad
                            timeSinceLastMovement += Time.deltaTime;
                        }

                        // Si ha pasado el tiempo de inactividad y no ha habido movimientos, resetear la rotaci�n
                        if (timeSinceLastMovement >= resetTime)
                        {
                            currentRotation = Quaternion.identity; // Restablece la rotaci�n acumulada
                            timeSinceLastMovement = 0f; // Reinicia el contador de tiempo
                        }

                        // Aplicar la rotaci�n acumulada al objeto
                        transform.rotation = currentRotation;
                    }
                    else
                    {
                        Debug.LogError("Error al parsear el valor de rotaci�n Y.");
                    }

                    // Guardar la rotaci�n actual para el pr�ximo ciclo
                    previousRotationY = smoothedRotationY;
                }
                else
                {
                    Debug.LogError("Los datos recibidos no tienen el formato esperado.");
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Tiempo de espera alcanzado en la lectura del puerto serie.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error en la lectura del puerto serie: " + ex.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Cerramos el puerto serie al salir
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
