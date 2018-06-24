using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Objeto/Poción")]
public class PociónSalud : Item {

    [SerializeField] int CantidadCurar;

    public override bool UsarObjeto()
    {
        if (AtributosJugador.atributosJugador.SaludActual == AtributosJugador.atributosJugador.Salud) {

            return false;
             }
        AtributosJugador.atributosJugador.SaludActual += CantidadCurar;
        return true;
    }

}
