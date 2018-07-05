using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Objeto/Poción")]
public class PocionSalud : Item
{
    public enum Pociones
    {
        Salud,
        Mana,
    }

    public Pociones tipo;
    [SerializeField] int CantidadCurar;
    public override bool UsarObjeto()
    {
        switch (tipo)
        {
            case Pociones.Salud:
                if (AtributosJugador.atributosJugador.SaludActual >= AtributosJugador.atributosJugador.Salud)
                {
                    return false;
                }
                AtributosJugador.atributosJugador.SaludActual += CantidadCurar;
                return true;

            case Pociones.Mana:
                if (AtributosJugador.atributosJugador.MagiaActual >= AtributosJugador.atributosJugador.Magia)
                {
                    return false;
                }
                AtributosJugador.atributosJugador.MagiaActual += CantidadCurar;
                return true;
            default:
                return false;
        }
    }
}
