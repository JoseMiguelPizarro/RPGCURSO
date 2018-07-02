using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.ComponentModel;

[System.Serializable]
public class UnityEventFloat : UnityEvent<float> { }
[System.Serializable]
public class UnityEventInt: UnityEvent<int> { }

public enum Atributos
{
    Salud,
    Magia,
    Velocidad,
    Inteligencia,
    Fuerza
}
public class AtributosJugador : Atacable {

    private ControlJugador jugador;
    public BarraPlayer barraDeSalud;
    public BarraPlayer barraDeMana;
    public BarraPlayer barraDeEXP;

    
    public UnityEventFloat OnCambiarSalud;
    public UnityEvent OnLevelUp;
    public UnityEventInt OnModificarPuntoAtributo;

    //Atributos iniciales

    //Atributos Base
        private int puntosAtributos;
        public int PuntosAtributos {
        get
        {
            return puntosAtributos;
        }
        set
        {
            puntosAtributos =value;
            OnModificarPuntoAtributo?.Invoke(puntosAtributos);
        }
        } //Puntos atributos para gastar habilidades
        public int SaludBase { get; set; }
        public int VelocidadBase { get; set; }
        public int MagiaBase { get; set; }
        public int InteligenciaBase { get; set; }
        public int FuerzaBase { get; set; }
        public int DefensaBase { get; set; }

    private int experienciaActual;
    private int magiaActual;
    private int saludActual;
    private float razonExpNivel;
    public int Experiencia
    {
        get {return experienciaActual; }

        set {
            experienciaActual =value;
            if (Nivel>1)
            {
                razonExpNivel = ((float)(Experiencia - CurvaExperienciaAcumulativa(Nivel))) / expSiguienteNivel;
                while (razonExpNivel>= 1)
                {
                    LevelUp();
                }
            }
            else
            {
                razonExpNivel = ((float)(experienciaActual)) / (expSiguienteNivel);
                while (razonExpNivel>= 1)
                {
                    LevelUp();
                }
            }
            ActualizarBarraEXP();
            PanelEstado.panelEstado.ActualizarTextos();
        } }

        public int Nivel { get; set; }

       
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
                OnCambiarSalud.Invoke((float)saludActual / Salud);
                Morir();
            }
            else
            {
                saludActual = value;
                OnCambiarSalud.Invoke((float)saludActual / Salud);
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


    public int ModificadorSalud { get; set; }
    public int ModificadorVelocidad { get; set; }
    public int ModificadorMagia { get; set; }
    public int ModificadorInteligencia { get; set; }
    public int ModificadorFuerza { get; set; }
    public int ModificadorDefensa { get; set; }

    public int Salud { get {return SaludBase+ModificadorSalud; } }
    public int Velocidad { get {return VelocidadBase+ModificadorVelocidad; } }
    public int Magia { get {return MagiaBase+ModificadorMagia; }  }
    public int Inteligencia { get {return InteligenciaBase+ModificadorInteligencia; } }
    public int Fuerza { get {return FuerzaBase+ModificadorFuerza; } }
    public int Defensa { get { return Defensa + ModificadorDefensa; } }


    public static AtributosJugador atributosJugador;

    private int expSiguienteNivel;


    private void Awake()
    {
        atributosJugador = this;
        //Setear StatsIniciales
    }
    private void Start()
    {
        atributosJugador.Inicializar();
        Debug.Log("valor acumulado al nivel 1"+CurvaExperienciaAcumulativa(1));
    }


    private void Inicializar()
    {
        VelocidadBase = 5;
        InteligenciaBase = 1;
        Nivel = 1;
        if (Nivel>1)
        {
            Experiencia = CurvaExperienciaAcumulativa(Nivel - 1);
        }
        else
        {
            Experiencia = 0;
        }
        ConfigurarSiguienteNivel();
        FuerzaBase = 1;
        jugador = GetComponent<ControlJugador>();
        jugador.velocidad = Velocidad;
        SaludBase = 15;
        MagiaBase = 5;
        DefensaBase = 0;
        magiaActual = Magia;
        saludActual = Salud;
        PuntosAtributos = Nivel-1;
        razonExpNivel = 0;
        PanelEstado.panelEstado.ActualizarTextos();
        ActualizarBarraEXP();
    }


    private void ActualizarBarraEXP()
    {
            barraDeEXP.Actualizar(razonExpNivel);
    }

    private void ActualizarBarraDeSalud()
    {
        barraDeSalud.Actualizar(saludActual / (float)Salud);
    }

    private void ActualizarBarraDeMana()
    {
        barraDeMana.Actualizar(magiaActual / (float)Magia);
    }


    public void ActualizarAtributos(List<Equipamiento> equipos)
    {
        ResetearModificadores();
        ActualizarModificadores(equipos);
        ActualizarBarraDeMana();
        OnCambiarSalud.Invoke((float)saludActual / Salud);
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
            ModificadorDefensa += equipo.Defensa;
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
        OnLevelUp?.Invoke();
        Nivel++;
        SaludActual = Salud;
        MagiaActual = Magia;
        ConfigurarSiguienteNivel();
        GenerartextHit("NUEVO NIVEL!", 2f, Color.cyan, 0.3f, new Vector2(0,0), new Vector2(0.5f, 0.5f));
        razonExpNivel = ((float)(Experiencia - CurvaExperienciaAcumulativa(Nivel))) / expSiguienteNivel;
       // PanelEstado.panelEstado.ActualizarTextos();
    }

    void ConfigurarSiguienteNivel()
    {
        PuntosAtributos += 1;
        expSiguienteNivel = CurvaExperiencia(Nivel); //Aproximar al valor superior
        Debug.Log("Expsiguiente para subir a nivel "+(Nivel+1)+" es " + expSiguienteNivel);
    }

    int CurvaExperiencia(int nivel)
    {
        return (Mathf.CeilToInt(50*(Mathf.Log(nivel, 3f)))+20);
    }

    int CurvaExperienciaAcumulativa(int nivel)
    {
        int exp = 0;
        for (int i = 1; i <nivel; i++)
        {
            exp += CurvaExperiencia(i);
        }
        return exp;
    }

    new void Morir()
    {
        Debug.Log("Jugador Murio");
        Destroy(gameObject);
    }

    public void ModificarAtributoBase(Atributos atributo, int cantidad)
    {
        switch (atributo)
        {
            case Atributos.Salud:
                SaludBase += cantidad;
                break;
            case Atributos.Magia:
                MagiaBase += cantidad;
                break;
            case Atributos.Velocidad:
                VelocidadBase += Velocidad;
                break;
            case Atributos.Inteligencia:
                InteligenciaBase += cantidad;
                break;
            case Atributos.Fuerza:
                FuerzaBase += cantidad;
                break;
        }
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void ModificarSaludBase(int cantidad)
    {
        SaludBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
        ActualizarBarraDeSalud();
    }

    public void ModificarMagiaBase(int cantidad)
    {
        MagiaBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
        ActualizarBarraDeMana();
    }

    public void ModificarVelocidadBase(int cantidad)
    {
        VelocidadBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void ModificarInteligenciaBase(int cantidad)
    {
        InteligenciaBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void ModificarDefensaBase(int cantidad)
    {
        DefensaBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void ModificarFuerzaBase(int cantidad)
    {
        FuerzaBase += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void ModificarPuntosDeAtributos(int cantidad)
    {
        PuntosAtributos += cantidad;
        PanelEstado.panelEstado.ActualizarTextos();
    }
}