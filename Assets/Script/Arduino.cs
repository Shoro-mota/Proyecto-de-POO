using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Arduino : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM3", 9600);
    public float rotacionFactor = 1.0f;
    private string[] datos;
    private float rotacionY;
    public bool boton;
    

    // Start is called before the first frame update
    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string value = serialPort.ReadLine();
                Debug.Log(value);
                datos = value.Split(','); // variable de datos del giroscopio
            }
            catch (System.Exception)
            {
                // throw;
            }
            if (datos.Length <= 8) {   // Si no hay datos, salimos
                if (float.TryParse(datos[3], out float rotationY))
                {
                    // Si la conversión es exitosa, lo usamos
                    rotacionY = rotationY;
                    transform.Rotate(0, 0, rotacionY* rotacionFactor);

                }
                if (float.TryParse(datos[7], out float button))
                {
                    // Si la conversión es exitosa, lo usamos
                    boton = (button == 0f);  // Si button es 0, boton será true, si es 1, será false porque en el arduino boton no presionado siempre es 1
                }
            }
        }
    }

    public bool Boton
    {
        get { return boton; }
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen) serialPort.Close();
    }
}
