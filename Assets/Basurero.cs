using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basurero : Casilla {

    public static Basurero basurero;
    private void Awake()
    {
        basurero = this;
    }
}
