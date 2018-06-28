using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HechizeroAI : EnemigoAI {

    public Proyectil proyectil;
	// Update is called once per frame
	void Update ()
    {
        CalcularDistanciaJugador();
        EnemigoComportamiento();
    }

    protected override void VoltearSprite()
    {
        if (direccion.x > 0)
        {
            sprite.flipX = true;
        }
        else { sprite.flipX = false; }
    }

    void InvocarBolaDeFuego()
    {
      Proyectil bolaDeFuego=  Instantiate(proyectil, transform.position,Quaternion.identity);
        bolaDeFuego.velocidad = 3;
        bolaDeFuego.trayectoria = GenerarDireccion();
    }
}
