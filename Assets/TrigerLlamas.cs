using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZonaFuego : MonoBehaviour
{
    public Vector3 posicionOrigen = new Vector3(1.47f, -0.03f, -10f);  // Posici�n inicial
    public GameObject skate;  // Referencia al objeto Skate

    // Detectar cuando el Skate entra en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == skate)  // Verifica si el objeto que toc� el trigger es el Skate
        {
            // Mueve el Skate a la posici�n de origen
            skate.transform.position = posicionOrigen;
        }
    }
}
