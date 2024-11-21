using UnityEngine;
using System; // Añadido para usar TimeoutException
using System.IO.Ports; // Para la comunicación serie

public class PlayerGyro : MonoBehaviour
{
    // Ajusta el puerto serie según el que estés usando
    SerialPort serialPort = new SerialPort("COM7", 9600);

    // Variable para almacenar la rotación acumulada en el eje Z
    private Quaternion currentRotation = Quaternion.identity; // Comienza en identidad (sin rotación)

    // Variable pública para ajustar la sensibilidad de la rotación
    public float sensitivity = 1.0f; // Sensibilidad inicial (puedes cambiarla en el Inspector de Unity)

    // Variables para suavizar la lectura del giroscopio
    private float previousRotationY = 0f; // Variable para la rotación anterior
    private float smoothedRotationY = 0f; // Variable para la rotación suavizada
    public float smoothingFactor = 0.1f; // Factor de suavizado (puedes ajustarlo)

    // Umbral para ignorar pequeñas fluctuaciones del giroscopio
    public float rotationThreshold = 0.01f; // Umbral mínimo de rotación para ser considerado

    // Tiempo de inactividad para resetear la rotación (en segundos)
    public float resetTime = 1.0f; // Tiempo después del cual se reseteará la rotación si no se detectan movimientos significativos
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
                print(value); // Mostrar datos para depuración

                // Dividir los datos en un array
                string[] data = value.Split(',');

                // Validar que la cadena contiene los 8 valores
                if (data.Length == 8)
                {
                    // Intentar convertir el valor del giro en el eje Y (índice 4 en los datos)
                    if (float.TryParse(data[4], out float rotationY))
                    {
                        // Si es la primera lectura, establecemos la referencia inicial
                        if (initialRotationY == 0f)
                        {
                            initialRotationY = rotationY;
                        }

                        // Restar la rotación inicial para eliminar el drift
                        float deltaRotationY = rotationY - initialRotationY;

                        // Aplicar suavizado con un filtro de media móvil
                        smoothedRotationY = Mathf.Lerp(previousRotationY, deltaRotationY, smoothingFactor);

                        // Usar el valor suavizado para la rotación
                        float adjustedRotationY = smoothedRotationY * sensitivity;

                        // Aplicar un umbral para evitar rotaciones pequeñas no deseadas
                        if (Mathf.Abs(adjustedRotationY) > rotationThreshold)
                        {
                            // Crear una rotación incremental, pero ahora la diferencia será negativa para invertir el giro
                            Quaternion rotationDelta = Quaternion.Euler(0, 0, -adjustedRotationY); // Resta para invertir la rotación

                            // Actualizar la rotación acumulada multiplicando por la nueva rotación
                            currentRotation *= rotationDelta;

                            // Resetear el tiempo desde el último movimiento significativo
                            timeSinceLastMovement = 0f;
                        }
                        else
                        {
                            // Si no hay movimiento significativo, incrementar el tiempo de inactividad
                            timeSinceLastMovement += Time.deltaTime;
                        }

                        // Si ha pasado el tiempo de inactividad y no ha habido movimientos, resetear la rotación
                        if (timeSinceLastMovement >= resetTime)
                        {
                            currentRotation = Quaternion.identity; // Restablece la rotación acumulada
                            timeSinceLastMovement = 0f; // Reinicia el contador de tiempo
                        }

                        // Aplicar la rotación acumulada al objeto
                        transform.rotation = currentRotation;
                    }
                    else
                    {
                        Debug.LogError("Error al parsear el valor de rotación Y.");
                    }

                    // Guardar la rotación actual para el próximo ciclo
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
