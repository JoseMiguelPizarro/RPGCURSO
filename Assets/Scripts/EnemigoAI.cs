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
    protected int stateHash;


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
        string hashstring = enemigo.nombre + "_muerto";
        stateHash = Animator.StringToHash(hashstring);
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

    public override void RecibirDanio(Transform atacante, int danio)
    {
        if (atacable)
        {
            Empujar(atacante);
            miSalud.SaludActual -= danio;
            if (enemigo.saludEnemigo.SaludActual <= 0)
            {
                enemigo.Dropear();
                StartCoroutine(Morir());
            }
        }
    }

    public override IEnumerator Morir()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        empujable = false;
        enemigo.muerto = true;
        Debug.Log("Muriendo");
        //  animator.Play("Caballero_muerto");
        animator.Play(stateHash, 0);
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }

    public virtual void EnemigoComportamiento()
    {
        if (enemigo.muerto == false)
        {
            if (!atacando && distanciaJugador < distanciaAtaque) //Atacar
            {
                GenerarDireccion(); //La dirección debe mantenerse mientras se realiza el ataque
                VoltearSprite();
                int rand = Random.Range(0, 100);
                animator.SetBool("Caminando", false);
                if (rand < 80)
                {
                    animator.SetTrigger("Atacar");
                }
            }
            else if ((!atacando && (enCombate || distanciaJugador <= distanciaDetectar)))
            {
                MoverHaciaJugador();
            }
            else
            {
                animator.SetBool("Caminando", false);
            }
           
        }
    }

    public virtual void SetAtacandoFalse()
    {
        atacando = false;
        Debug.Log("Atacando falso");
    }

    public virtual void SetAtacandoTrue()
    {
        atacando = true;
        Debug.Log("Atacando true");
    }
}

