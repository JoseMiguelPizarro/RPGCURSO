using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsarSkill : Skills {

    public int costeMagiaDash;
    public int costeMagiaBolaDeFuego;

    public void UsarDash(Vector2 dirección)
    {
        if (AtributosJugador.atributosJugador.MagiaActual>=costeMagiaDash)
        {
            AtributosJugador.atributosJugador.MagiaActual -= costeMagiaDash;
            StartCoroutine(Dash(dirección));
        }
    }

    public void UsarBolaDeFuego(int inteligencia, Vector2 trayectoria, Rigidbody2D rb)
    {
        if (AtributosJugador.atributosJugador.MagiaActual >= costeMagiaBolaDeFuego)
        {
            AtributosJugador.atributosJugador.MagiaActual -= costeMagiaBolaDeFuego;
            BolaDeFuego(inteligencia, trayectoria, rb);
        }
    }
}
