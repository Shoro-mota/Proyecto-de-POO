using UnityEngine;
using System; // A�adido para usar TimeoutException
using System.IO.Ports; // Para la comunicaci�n serie

public class PlayerGyro : MonoBehaviour
{
    // Ajusta el puerto serie seg�n el que est�s usando
    SerialPort serialPort = new SerialPort("COM7", 9600);

    // Variable p�blica para ajustar la sensibilidad de la rotaci�n
    public float sensitivity = 1.0f; // Sensibilidad inicial (puedes cambiarla en el Inspector de Unity)

    void Start()
    {
        // Abrimos el puerto serie
        serialPort.Open();
        serialPort.ReadTimeout = 100; // Tiempo de espera en ms
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
                        // Ajustar la sensibilidad multiplicando por el factor de sensibilidad
                        float adjustedRotationY = rotationY * sensitivity;

                        // Aplicar la rotaci�n al objeto utilizando un quaternion.
                        // La rotaci�n ahora solo depende de la lectura del giroscopio en el eje Y.
                        // El eje Z de Unity se actualiza con el valor ajustado de rotaci�nY.
                        transform.rotation = Quaternion.Euler(0, 0, adjustedRotationY); // Rota solo en el eje Z
                    }
                    else
                    {
                        Debug.LogError("Error al parsear el valor de rotaci�n Y.");
                    }
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
