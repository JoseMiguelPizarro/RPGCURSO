using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtributosJugador : MonoBehaviour {

	public int Salud { get; set; }
    public int Velocidad { get; set; }
    public int Magia { get; set; }
    public int Fuerza { get; set; }
    public int Experiencia { get; set; }
    public int Nivel { get; set; }


    public static AtributosJugador atributosJugador;

    private void Awake()
    {
        atributosJugador = this;
    }

    AtributosJugador()
    {
        Salud = 10;
        Velocidad = 1;
        Magia = 5;
        Experiencia = 0;
        Nivel = 1;
        Fuerza = 1;
    }
}
