using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGyroscope : MonoBehaviour

{
    public IDeviceReader deviceReader;  // Referencia al script que lee los datos del giroscopio
    private string[] gyroData;  // Datos del giroscopio
    

    // Start is called before the first frame update
    void Start()
    {
        if (deviceReader == null)
        {
            Debug.LogError("DeviceReader no está asignado, Revisar COM (USB).");
        }
    }

    // Update is called once per frame
    void Update()
    {
        gyroData = deviceReader.GetGyroData();  // Obtenemos los datos del giroscopio
    }

    public string[] GetGyroData()
    {
        return gyroData;  // Retorna los datos obtenidos
    }
}
