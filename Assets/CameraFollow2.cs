using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlador : MonoBehaviour
{
    public GameObject personaje;  // El objeto a seguir
    private Vector3 posicionRelativa;
    public float ejeYFijo;        // Altura fija de la c�mara en el eje Y
    public float smoothSpeed = 0.125f; // Velocidad de suavizado

    // Use this for initialization
    void Start()
    {
        // Calcula la posici�n relativa inicial
        posicionRelativa = transform.position - personaje.transform.position;

        // Asigna el eje Y actual como el fijo, si no se especifica uno
        ejeYFijo = transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calcula la nueva posici�n manteniendo el eje Y fijo
        Vector3 nuevaPosicion = personaje.transform.position + posicionRelativa;
        nuevaPosicion.y = ejeYFijo;

        // Suaviza la transici�n de la c�mara
        transform.position = Vector3.Lerp(transform.position, nuevaPosicion, smoothSpeed);
    }
}
