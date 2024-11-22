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
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // Ajuste progresivo del eje Z a 0
            float currentZ = transform.rotation.eulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f; // Ajustamos el rango [-180, 180]
            float adjustedZ = Mathf.MoveTowards(currentZ, 0f, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, adjustedZ);
        }
        else
        {
            // Lectura del giroscopio
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
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen) serialPort.Close();
    }
}
