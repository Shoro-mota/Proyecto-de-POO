using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeHandler : MonoBehaviour  
{
    public IGyroscope igyroscope;  // Referencia al script que lee los datos del giroscopio
    [HideInInspector] public float rotacionY;  // Valor de la rotación en Y
    [HideInInspector] public bool boton;  // Valor del botón
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string[] gyroData = igyroscope.GetGyroData();
        if (float.TryParse(gyroData[3], out float rotationY))
        {
            // Si la conversión es exitosa, lo usamos
            rotacionY = rotationY;
           
        }
        if (float.TryParse(gyroData[7], out float button))
        {
            // Si la conversión es exitosa, lo usamos
            boton = (button == 0f);  // Si button es 0, boton será true, si es 1, será false porque en el arduino boton no presionado siempre es 1
        }

    }

    public float GetRotacion()
    {
        return rotacionY;  // Exponemos los datos a otros scripts
    }

    public bool GetBoton()
    {
        return boton;  // Exponemos los datos a otros scripts
    }

}
