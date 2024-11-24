using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGyroscope : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public interface ObtenerDatosGiroscopio  // Interfaz para obtener los datos del giroscopio
    {
        string[] GetGyroData();  // Método para obtener los datos del giroscopio
    }

}
