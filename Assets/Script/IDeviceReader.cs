using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;  

public class IDeviceReader : MonoBehaviour
{
    private string[] datos;
    SerialPort serialPort = new SerialPort("COM3", 9600);

    // Start is called before the first frame update
    void Start()
    {
        serialPort.Open();
        serialPort.ReadTimeout = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort.IsOpen) {
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
        }
    }

    public string[] GetGyroData()
    {
        return datos;  // Exponemos los datos a otros scripts
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen) serialPort.Close();
    }
}
