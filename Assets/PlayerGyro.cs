using UnityEngine;
using System;
using System.IO.Ports;

public class PlayerGyro : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM3", 9600);

    [Header("Gyroscope Settings")]
    public float sensitivity = 1.0f;
    public float smoothingFactor = 0.1f;
    public float rotationThreshold = 0.01f;
    public float resetTime = 1.0f;

    private float smoothedRotationY = 0f;
    private float previousRotationY = 0f;
    private float timeSinceLastMovement = 0f;
    private float initialRotationY = 0f;

    [Header("Output")]
    public float currentInclination; // Valor expuesto para otros scripts

    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
    }

    void Update()
    {
        // Si la tecla CTRL está presionada, se resetean todos los ejes a 0
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Mantenemos la rotación en 0 mientras CTRL está presionado
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            // Si no se presiona CTRL, leemos los datos del giroscopio
            if (serialPort.IsOpen)
            {
                try
                {
                    string value = serialPort.ReadLine();
                    string[] data = value.Split(',');

                    // Solo procesamos si los datos tienen el formato esperado
                    if (data.Length == 8 && float.TryParse(data[3], out float rotationY))
                    {
                        if (initialRotationY == 0f)
                        {
                            initialRotationY = rotationY;
                        }

                        float deltaRotationY = rotationY - initialRotationY;
                        smoothedRotationY = Mathf.Lerp(previousRotationY, deltaRotationY, smoothingFactor);

                        if (Mathf.Abs(smoothedRotationY) > rotationThreshold)
                        {
                            currentInclination = smoothedRotationY * sensitivity;
                            timeSinceLastMovement = 0f;
                        }
                        else
                        {
                            timeSinceLastMovement += Time.deltaTime;
                        }

                        if (timeSinceLastMovement >= resetTime)
                        {
                            currentInclination = 0f;
                            timeSinceLastMovement = 0f;
                        }

                        previousRotationY = smoothedRotationY;
                    }
                }
                catch (TimeoutException) { }
            }

            // Aplicamos la rotación calculada por el giroscopio (cuando no se presiona CTRL)
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentInclination);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
