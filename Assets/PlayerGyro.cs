using UnityEngine;
using System;
using System.IO.Ports;
using System.Collections;  // Agregado para IEnumerator

public class PlayerGyro : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM3", 9600);

    public bool isButtonPressed { get; set; }
    [Header("Gyroscope Settings")]
    public float sensitivity = 0.5f;
    public float smoothingFactor = 0.1f; // Suavizado más sutil para evitar lag
    public float rotationThreshold = 0.125f;
    public float resetTime = 1.0f;
    public float readInterval = 0.05f; // Más frecuente, pero con menor impacto
    private float smoothedRotationY = 0f;
    private float previousRotationY = 0f;
    private float timeSinceLastMovement = 0f;
    private float initialRotationY = 0f;

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

                if (data.Length == 8 && float.TryParse(data[3], out float rotationY))
                {
                    if (initialRotationY == 0f) initialRotationY = rotationY;

                    float deltaRotationY = rotationY - initialRotationY;
                    deltaRotationY = Mathf.Clamp(deltaRotationY, -180f, 180f);

                    // Suavizado exponencial para rotación más suave
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
                if (data.Length >= 8)
                {
                    if (int.TryParse(data[7], out int buttonState))
                    {
                        isButtonPressed = buttonState == 0 ? true : false; // Asigna true si el valor es 1
                        Debug.Log($"Estado del botón: {isButtonPressed}");
                        if (isButtonPressed)
                        {
                            Debug.Log("Botón presionado");
                        }
                        else
                        {
                            Debug.Log("Botón liberado");
                        }
                    }
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
