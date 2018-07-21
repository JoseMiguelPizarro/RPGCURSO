using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GestorDeSalud : MonoBehaviour {

    public BarraDeSalud barraDeSalud;
    public int salud;
    public UnityEventFloat OnCambiarSalud;
    
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
            OnCambiarSalud?.Invoke(saludActual / (float)salud);
            Actualizar();
        } }
    private void Awake()
    {
        saludActual = salud;
    }
    public void Actualizar()
    {
        if (OnCambiarSalud==null)
        {
            barraDeSalud.Actualizar((saludActual / (float)salud));
        }
    }
}
