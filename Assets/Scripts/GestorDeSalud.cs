using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeSalud : MonoBehaviour {

    public BarraDeSalud barraDeSalud;
    public int salud;
    [HideInInspector]
    private int saludActual;
    public int SaludActual {
        get {
            return saludActual;
        }
        set {
            saludActual =value;
            if (saludActual<0)
            {
                saludActual = 0;
            }
            Actualizar();
        } }
    private void Awake()
    {
        saludActual = salud;
    }
    public void Actualizar()
    {
        barraDeSalud.Actualizar((saludActual / (float)salud));
    }
}
