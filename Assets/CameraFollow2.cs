using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlador : MonoBehaviour
{
    public GameObject personaje;  // El objeto a seguir
    private Vector3 posicionRelativa;
    public float ejeYFijo;        // Altura fija de la cámara en el eje Y

    // Use this for initialization
    void Start()
    {
        // Calcula la posición relativa inicial
        posicionRelativa = transform.position - personaje.transform.position;

        // Asigna el eje Y actual como el fijo, si no se especifica uno
        ejeYFijo = transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calcula la nueva posición manteniendo el eje Y fijo
        Vector3 nuevaPosicion = personaje.transform.position + posicionRelativa;
        nuevaPosicion.y = ejeYFijo;

        // Actualiza la posición de la cámara
        transform.position = nuevaPosicion;
    }
}
