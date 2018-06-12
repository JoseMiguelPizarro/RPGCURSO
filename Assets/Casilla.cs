using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour {

    public ObjetoInventario objetoInventario;
    private void Start()
    {
        AñadirObjetoCasilla();
    }

    public void AñadirObjetoCasilla()
    {
        objetoInventario = GetComponentInChildren<ObjetoInventario>();
    }
}
