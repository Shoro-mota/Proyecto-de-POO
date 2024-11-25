using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector] public GyroscopeHandler gyroscopeHandler;  // Referencia al script que lee los datos del giroscopio
    public float rotacionFactor = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, gyroscopeHandler.GetRotacion() * rotacionFactor);
    }


}
