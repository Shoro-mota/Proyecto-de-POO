using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public Vector3 posicionOrigen = new Vector3(1.47f, -0.03f, -10f); // Posición inicial del Skate

    void Update()
    {
        // Detecta si se presiona la tecla R
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reinicia la posición del objeto a la posición de origen
            transform.position = posicionOrigen;
        }
    }
}

