using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Arduino : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM5", 9600);
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
            if (datos.Length <= 8) // el array tiene al menos 8 elementos
            {
                if (float.TryParse(datos[1], out float rotationY))
                {
                    rotacionY = rotationY;
                    transform.Rotate(0, 0, (rotacionY * rotacionFactor)*-1);
                    
                }
                if (float.TryParse(datos[7], out float button))
                {
                    boton = (button == 0f); // 0 significa botón presionado
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
