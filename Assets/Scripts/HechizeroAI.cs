using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Skills))]
[RequireComponent(typeof(Enemigo))]
public class HechizeroAI : EnemigoAI {

    public float velocidadProyectil;
    public float fuerzaRetroceso = 5f;
	// Update is called once per frame
	void Update ()
    {
        CalcularDistanciaJugador();
        EnemigoComportamiento();
    }
   
    protected override void VoltearSprite()
    {
        if (direccionAtaque.x > 0)
        {
            sprite.flipX = true;
        }
        else { sprite.flipX = false; }
    }

    void InvocarBolaDeFuego()
    {

     skills.BolaDeFuego(enemigo.Inteligencia, direccionAtaque,rb);
      //Proyectil bolaDeFuego=  Instantiate(proyectil, transform.position,Quaternion.identity);
      //  bolaDeFuego.tagObjetivo = "Player";
      //  bolaDeFuego.daño = enemigo.Inteligencia;
      //  bolaDeFuego.velocidad = velocidadProyectil;
      //  bolaDeFuego.trayectoria = direccionAtaque;
      //  float anguloRotacion = Mathf.Atan2(direccionAtaque.y, direccionAtaque.x)*Mathf.Rad2Deg;
      //  bolaDeFuego.transform.Rotate(0,0,anguloRotacion);
      //  RetrocederRb();
    }

    IEnumerator Retroceso()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.position = transform.position - (Vector3)direccionAtaque.normalized*10* Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void RetrocederRb()
    {
        GetComponent<Rigidbody2D>().velocity=((transform.position - (Vector3)direccionAtaque).normalized*fuerzaRetroceso);
    }
}
