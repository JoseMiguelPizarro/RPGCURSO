using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtributosJugador : Atacable {

    public Text textoSalud;
    private ControlJugador jugador;
    public BarraPlayer barraDeSalud;
    public BarraPlayer barraDeMana;
    public BarraPlayer barraDeEXP;

    //Atributos iniciales
   
    //Atributos Base
        public int SaludBase { get; set; }
        public int VelocidadBase { get; set; }
        public int MagiaBase { get; set; }
        public int InteligenciaBase { get; set; }
        public int FuerzaBase { get; set; }
        public int Experiencia
    {
        get {return experienciaActual; }

        set {experienciaActual=value;
            if ((float)(experienciaActual)/(ExpSiguienteNivel)==1)
            {
                LevelUp();
            }
            ActualizarBarraEXP();
        } }

        public int Nivel { get; set; }

        private int experienciaActual;
        private int magiaActual;
        private int saludActual;
        public int SaludActual
    {
        get
        {
            return saludActual;
        }
        set
        {
            
            if (value > SaludBase)
            {
                saludActual = Salud;

            }
            else if (value <= 0)
            {
                saludActual = 0;
                ActualizarBarraDeSalud();
                Morir();
            }
            else
            {
                saludActual = value;
                ActualizarBarraDeSalud();
            }
        }
    }
        public int MagiaActual
    {
        get { return magiaActual; }
        set
        {
            if (value > Magia)
            {
                magiaActual = Magia;
               
            }
            else if (value<=0)
            {
                magiaActual = 0;
            }
            else
            {
                magiaActual = value;
            };
            ActualizarBarraDeMana();
        }
    }

    private int ExpSiguienteNivel = 20;

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
        //Setear StatsIniciales
    }
    private void Start()
    {
        atributosJugador.Inicializar();
    }

    private void Inicializar()
    {
        VelocidadBase = 5;
        InteligenciaBase = 1;
        Experiencia = 0;
        Nivel = 1;
        FuerzaBase = 1;
        jugador = GetComponent<ControlJugador>();
        jugador.velocidad = Velocidad;
        SaludBase = 10;
        MagiaBase = 5;
        magiaActual = Magia;
        saludActual = Salud;
        PanelEstado.panelEstado.ActualizarTextos();
    }


    private void ActualizarBarraEXP()
    {
        barraDeEXP.Actualizar((float)Experiencia /ExpSiguienteNivel);
    }

    private void ActualizarBarraDeSalud()
    {
        barraDeSalud.Actualizar(saludActual / (float)Salud);
    }

    private void ActualizarBarraDeMana()
    {
        barraDeMana.Actualizar(magiaActual / (float)Magia);
    }

    private void ActualizarSaludActual()
    {
        textoSalud.text = SaludActual.ToString();
    }

    public void ActualizarAtributos(List<Equipamiento> equipos)
    {
        ResetearModificadores();
        ActualizarModificadores(equipos);
        ActualizarBarraDeMana();
        ActualizarBarraDeSalud();
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
        GenerartextHit(daño.ToString());
        SaludActual -= daño;
    }

    void LevelUp()
    {
        Nivel++;
        ConfigurarSiguienteNivel();
        Experiencia = 0;
        PanelEstado.panelEstado.ActualizarTextos();
    }

    void ConfigurarSiguienteNivel()
    {
        ExpSiguienteNivel = (int)Mathf.Log(Nivel, 3f);
    }

     new void Morir()
    {
        Debug.Log("Jugador Murio");
        Destroy(gameObject);
    }
}
