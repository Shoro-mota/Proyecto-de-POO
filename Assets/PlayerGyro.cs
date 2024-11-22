using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections;  // Agregado para IEnumerator

public class PlayerGyro : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM7", 9600);

    [Header("Gyroscope Settings")]
    public float sensitivity = 1.0f;
    public float smoothingFactor = 0.05f; // Suavizado más sutil para evitar lag
    public float rotationThreshold = 0.01f;
    public float resetTime = 1.0f;
    public float readInterval = 0.05f; // Más frecuente, pero con menor impacto
    private float smoothedRotationY = 0f;
    private float previousRotationY = 0f;
    private float timeSinceLastMovement = 0f;
    private float initialRotationY = 0f;

    public bool isButtonPressed;

    [Header("Output")]
    public float currentInclination; // Valor expuesto para otros scripts

    private bool canReadGyro = true;  // Variable para controlar la lectura

    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
        StartCoroutine(ReadGyroDataCoroutine());
    }

    // Coroutine para leer los datos en intervalos
    IEnumerator ReadGyroDataCoroutine()
    {
        while (true)
        {
            if (canReadGyro)
            {
                ReadGyroData();
            }

            yield return new WaitForSeconds(readInterval); // Espera según el intervalo
        }
    }

    void ReadGyroData()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string value = serialPort.ReadLine();
                string[] data = value.Split(',');

                if (data.Length == 8) // Asegurarse de que la longitud sea exactamente 8
                {
                    if (float.TryParse(data[3], out float rotationY))
                    {
                        // Procesar rotación
                        if (initialRotationY == 0f) initialRotationY = rotationY;

                        float deltaRotationY = rotationY - initialRotationY;
                        deltaRotationY = Mathf.Clamp(deltaRotationY, -180f, 180f);

                        smoothedRotationY = Mathf.Lerp(smoothedRotationY, deltaRotationY, smoothingFactor);

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

                    if (int.TryParse(data[7], out int buttonState)) // Cambiado de data[8] a data[7]
                    {
                        isButtonPressed = buttonState == 1; // Asigna true si es 1
                    }
                }
                else
                {
                    Debug.LogWarning($"Datos inesperados recibidos: {value}");
                }

            }
            catch (TimeoutException) { }
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Ajuste progresivo del eje Z a 0
            float currentZ = transform.rotation.eulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f; // Ajustamos el rango [-180, 180]
            float adjustedZ = Mathf.MoveTowards(currentZ, 0f, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, adjustedZ);
        }
        // Aquí ya no se realiza procesamiento del giroscopio en cada frame
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen) serialPort.Close();
    }
}
