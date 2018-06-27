using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class Atacante : MonoBehaviour {


    //call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
    public Collider2D LastHit { get { return golpe; } }
    public float offset = 1f;
    public Vector2 tamaño = new Vector2(1f, 1f);
    public LayerMask hittableLayers;

    protected ContactFilter2D filtroAtaque;
    protected Collider2D[] AtaqueOverlapResults = new Collider2D[10];
    protected Transform AtacanteTransform;
    protected Collider2D golpe;

    public Vector2 direcciónAtaque;
    Vector2 escala = new Vector2();
    Vector2 facingOffset = new Vector2();
    Vector2 tamañoEscalado = new Vector2();
    Vector2 puntoA = new Vector2();
    Vector2 puntoB = new Vector2();

    void Awake()
    {
        filtroAtaque.layerMask = hittableLayers;
        filtroAtaque.useLayerMask = true;
        AtacanteTransform = transform;
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, facingOffset, Color.yellow);
        Debug.DrawLine(puntoA, puntoB, Color.red);
    }


    public void Atacar(Vector2 mirada, int daño)
    {
        Debug.Log("Atacó");
        escala = AtacanteTransform.lossyScale;

        facingOffset = Vector2.Scale(offset * mirada.normalized, escala);

        tamañoEscalado = Vector2.Scale(tamaño, escala);

        puntoA = (Vector2)AtacanteTransform.position + facingOffset - tamañoEscalado * 0.5f;
        puntoB = puntoA + tamañoEscalado;

        int hitCount = Physics2D.OverlapArea(puntoA, puntoB, filtroAtaque, AtaqueOverlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            golpe = AtaqueOverlapResults[i];
            Enemigo enemigo = golpe.GetComponent<Enemigo>();

            if (enemigo)
            {
                Debug.Log("Dañó a " + enemigo.name);
                enemigo.RecibirDaño(transform, daño);
            }
        }
    }

    public void Atacar(Mirada mirada, int daño)
    {
        Debug.Log("Atacó");
        DeterminarDirecciónAtaque(mirada);
         escala = AtacanteTransform.lossyScale;

         facingOffset = Vector2.Scale(offset*direcciónAtaque, escala);

         tamañoEscalado = Vector2.Scale(tamaño, escala);

         puntoA = (Vector2)AtacanteTransform.position + facingOffset - tamañoEscalado * 0.5f;
         puntoB = puntoA + tamañoEscalado;

        int hitCount = Physics2D.OverlapArea(puntoA, puntoB, filtroAtaque, AtaqueOverlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            golpe = AtaqueOverlapResults[i];
           Enemigo enemigo = golpe.GetComponent<Enemigo>();

            if (enemigo)
            {
                Debug.Log("Dañó a "+enemigo.name);
                enemigo.RecibirDaño(transform,daño);
            }
        }
        
    }

   public void DeterminarDirecciónAtaque(Mirada mirada)
    {
        switch (mirada)
        {
            case Mirada.Arriba:
                direcciónAtaque = Vector2.up;
                break;
            case Mirada.Abajo:
                direcciónAtaque = Vector2.down;
                break;
            case Mirada.Izquierda:
                direcciónAtaque = Vector2.left;
                break;
            case Mirada.Derecha:
                direcciónAtaque = Vector2.right;
                break;
            default:
                direcciónAtaque = Vector2.down;
                break;
        }
    }
}
