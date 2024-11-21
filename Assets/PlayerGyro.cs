using UnityEngine;
using System;
using System.IO.Ports; // Para la comunicación serie

public class PlayerGyro : MonoBehaviour
{
    // Ajusta el puerto serie según el que estés usando
    SerialPort serialPort = new SerialPort("COM3", 9600);

    private Quaternion currentRotation = Quaternion.identity; // Comienza en identidad (sin rotación)
    public float sensitivity = 1.0f; // Sensibilidad inicial (puedes cambiarla en el Inspector de Unity)
    private float previousRotationY = 0f; // Variable para la rotación anterior
    private float smoothedRotationY = 0f; // Variable para la rotación suavizada
    public float smoothingFactor = 0.1f; // Factor de suavizado (puedes ajustarlo)
    public float rotationThreshold = 0.01f; // Umbral mínimo de rotación para ser considerado
    public float resetTime = 1.0f; // Tiempo después del cual se reseteará la rotación
    private float timeSinceLastMovement = 0f;
    private float initialRotationY = 0f;

    void Start()
    {
        // Abrimos el puerto serie
        serialPort.Open();
        serialPort.ReadTimeout = 100; // Tiempo de espera en ms
        initialRotationY = 0f;
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string value = serialPort.ReadLine();
                print(value); // Mostrar datos para depuración

                string[] data = value.Split(',');

                if (data.Length == 8)
                {
                    if (float.TryParse(data[3], out float rotationY))
                    {
                        if (initialRotationY == 0f)
                        {
                            initialRotationY = rotationY;
                        }

                        float deltaRotationY = rotationY - initialRotationY;
                        smoothedRotationY = Mathf.Lerp(previousRotationY, deltaRotationY, smoothingFactor);
                        float adjustedRotationY = smoothedRotationY * sensitivity;

                        if (Mathf.Abs(adjustedRotationY) > rotationThreshold)
                        {
                            Quaternion rotationDelta = Quaternion.Euler(0, 0, adjustedRotationY);
                            currentRotation *= rotationDelta;
                            timeSinceLastMovement = 0f;
                        }
                        else
                        {
                            timeSinceLastMovement += Time.deltaTime;
                        }

                        if (timeSinceLastMovement >= resetTime)
                        {
                            currentRotation = Quaternion.identity;
                            timeSinceLastMovement = 0f;
                        }

                        transform.rotation = currentRotation;
                    }
                    else
                    {
                        Debug.LogError("Error al parsear el valor de rotación Y.");
                    }

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
