using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HechizeroAI : EnemigoAI {

	
	// Update is called once per frame
	void Update ()
    {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
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
}
