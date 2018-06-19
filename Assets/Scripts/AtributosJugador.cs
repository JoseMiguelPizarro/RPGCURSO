using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtributosJugador : MonoBehaviour {

    public Text textoSalud;


        public int Salud { get; set; }
        public int Velocidad { get; set; }
        public int Magia { get; set; }
        public int Inteligencia { get; set; }
        public int Fuerza { get; set; }
        public int Experiencia { get; set; }
        public int Nivel { get; set; }
   

    private int saludActual;
    public int SaludActual
    {
        get { return saludActual; }
        set
        {
            if (value > Salud)
            {
                saludActual = Salud;
            }
            else saludActual = value;
        }
    }
    //public int MagiaActual {
    //    get {return MagiaActual; }
    //    set
    //    {
    //        if (MagiaActual+value>Magia)
    //        {
    //            MagiaActual = Magia;
    //        }; } }


    public static AtributosJugador atributosJugador;

    private void Awake()
    {
        atributosJugador = this;
    }

    

    AtributosJugador()
    {
        Salud = 10;
        SaludActual = 2;
        Velocidad = 1;
        Magia = 5;
        //MagiaActual = Magia;
        Experiencia = 0;
        Nivel = 1;
        Fuerza = 1;
    }
    private void Update()
    {
        textoSalud.text = SaludActual.ToString();
    }
}
