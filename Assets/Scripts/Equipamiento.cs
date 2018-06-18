using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TipoDeEquipamiento
{
    Arma,
    Ropaje,
    Botas,
    Accesorio,
    Casco,
    Guantes,
}

[CreateAssetMenu(menuName = "Objeto/Equipamiento")]
public class Equipamiento : Item {

    public TipoDeEquipamiento tipoEquipo;
    [Space]
    [Tooltip("Salud que entrega el objeto")]
    public int Salud;
    public int Magia;
    public int Inteligencia;
    public int Fuerza;
    public int Defensa;
    public int Velocidad;
}
