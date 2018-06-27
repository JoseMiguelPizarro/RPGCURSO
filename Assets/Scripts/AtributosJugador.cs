using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtributosJugador : Daniable {

    public Text textoSalud;
    public ControlJugador jugador;
    //Atributos Base
        public int SaludBase { get; set; }
        public int VelocidadBase { get; set; }
        public int MagiaBase { get; set; }
        public int InteligenciaBase { get; set; }
        public int FuerzaBase { get; set; }
        public int Experiencia { get; set; }
        public int Nivel { get; set; }

        private int magiaActual;
        private int saludActual;
        public int SaludActual
    {
        get { return saludActual; }
        set
        {
            if (value > SaludBase)
            {
                saludActual = SaludBase;
            }
            else saludActual = value;
        }
    }
        public int MagiaActual
    {
        get { return MagiaActual; }
        set
        {
            if (value > MagiaBase)
            {
                MagiaActual = MagiaBase;
            };
        }
    }

    public int ModificadorSalud { get; set; }
    public int ModificadorVelocidad { get; set; }
    public int ModificadorMagia { get; set; }
    public int ModificadorInteligencia { get; set; }
    public int ModificadorFuerza { get; set; }

    public int Salud { get {return SaludBase+ModificadorSalud; } }
    public int Velocidad { get {return VelocidadBase+ModificadorVelocidad; } }
    public int Magia { get {return MagiaBase+ModificadorMagia; }  }
    public int Inteligencia { get {return InteligenciaBase+ModificadorInteligencia; } }
    public int Fuerza { get {return FuerzaBase+ModificadorFuerza; } }


    public static AtributosJugador atributosJugador;

    private void Awake()
    {
        atributosJugador = this;
        jugador.velocidad = Velocidad;
    }


    AtributosJugador()
    {
        SaludBase = 10;
        SaludActual = 2;
        VelocidadBase = 5;
        MagiaBase = 5;
        InteligenciaBase = 1;
        Experiencia = 0;
        Nivel = 1;
        FuerzaBase = 1;
    }

    private void Update()
    {
        ActualizarSaludActual();
    }

    private void ActualizarSaludActual()
    {
        textoSalud.text = SaludActual.ToString();
    }

    public void ActualizarAtributos(List<Equipamiento> equipos)
    {
        ResetearModificadores();
        ActualizarModificadores(equipos);
        jugador.velocidad = Velocidad;
    }

    private void ActualizarModificadores(List<Equipamiento> equipos)
    {
        foreach (Equipamiento equipo in equipos)
        {
            ModificadorSalud += equipo.Salud;
            ModificadorMagia += equipo.Magia;
            ModificadorInteligencia += equipo.Inteligencia;
            ModificadorVelocidad += equipo.Velocidad;
            ModificadorFuerza += equipo.Fuerza;
        }
    }

    private void ResetearModificadores()
    {
        ModificadorSalud = 0;
        ModificadorMagia = 0;
        ModificadorInteligencia = 0;
        ModificadorFuerza = 0;
        ModificadorVelocidad = 0;
    }

    public override void RecibirDanio(Transform atacante, int daño)
    {
        Debug.Log("Jugador Dañado");
        Empujar(atacante);
        SaludActual -= daño;
    }
    
}
