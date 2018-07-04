using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorMateriales : MonoBehaviour {

    public static GestorMateriales gestorMateriales;
    public  Material[] materiales;

    private void Start()
    {
        gestorMateriales = this;
    }
}
