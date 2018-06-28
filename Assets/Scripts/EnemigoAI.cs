using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoAI : Atacable {

    protected bool enCombate = false;
    protected bool atacando = false;
    public float distanciaDetectar = 6f;
    public float distanciaAtaque = 2.5f;
    protected float distanciaJugador;
    protected Animator animator;
    protected Vector2 direccion;
    protected Enemigo enemigo;
    public Transform jugador;
    protected SpriteRenderer sprite;
    protected Atacante atacante;
    protected GestorDeSalud miSalud;


    private void Awake()
    {
        InicializarComponentes();
    }

    protected void InicializarComponentes()
    {
        animator = GetComponent<Animator>();
        enemigo = GetComponent<Enemigo>();
        sprite = GetComponent<SpriteRenderer>();
        atacante = GetComponent<Atacante>();
        miSalud = GetComponent<GestorDeSalud>();
    }

    protected virtual void MoverHaciaJugador()
    {
        GenerarDireccion();
        enCombate = true;
        animator.SetBool("Caminando", true);
        //transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
        transform.position += (Vector3)direccion.normalized * enemigo.Velocidad * Time.deltaTime;
        VoltearSprite();
    }

    protected virtual void GenerarDireccion()
    {
        direccion = jugador.position - transform.position;
    }

    protected virtual void VoltearSprite()
    {
        if (direccion.x < 0)
        {
            sprite.flipX = true;
        }
        else { sprite.flipX = false; }
    }

    public virtual void Atacar()
    {
        atacante.Atacar(direccion, enemigo.Fuerza);
    }
}
