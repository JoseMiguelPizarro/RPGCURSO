using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaballeroAI : Daniable {

	public Transform jugador;
    private Animator animator;
    private Atacante atacante;
    private Enemigo enemigo;
    private float distanciaJugador;
    private Vector2 direccion;
    private AnimatorStateInfo animInfo;
    private SpriteRenderer sprite;
    private GestorDeSalud miSalud;
    // Update is called once per frame
    private void Awake()
    {
        atacante = GetComponent<Atacante>();
        animator = GetComponent<Animator>();
        enemigo = GetComponent<Enemigo>();
        sprite = GetComponent<SpriteRenderer>();
        miSalud = GetComponent<GestorDeSalud>();
    }
    void Update()
    {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        direccion = jugador.position - transform.position;
        animInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (enemigo.saludEnemigo.SaludActual<=0)
        {
            StartCoroutine(Morir());
        }
        if (enemigo.muerto == false)
        {
            if (distanciaJugador < 2)
            {
                animator.SetBool("Caminando", false);
                animator.SetTrigger("Atacar");
            }
            else if (!animInfo.IsTag("Ataque") && distanciaJugador <= 6)
            {
                animator.SetBool("Caminando", true);
                //transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
                transform.position += (Vector3)direccion.normalized * 3 * Time.deltaTime;
                if (direccion.x < 0)
                {
                    sprite.flipX = true;
                }
                else { sprite.flipX = false; }
            }
            else
            {
                animator.SetBool("Caminando", false);
            }
        }
    }

    public void GuardiaAtaque()
    {
        Debug.Log("GuardiaAtacando");
        transform.position = Vector3.MoveTowards(transform.position, transform.position+ (Vector3)direccion.normalized*3, 0.5f);
        atacante.Atacar(direccion, enemigo.Fuerza);
    }


    public override void RecibirDanio(Transform atacante, int danio)
    {
        Empujar(atacante);
        miSalud.SaludActual -= danio;
        if (enemigo.saludEnemigo.SaludActual<=0)
        {
            StartCoroutine(Morir());
        }
    }

    public override IEnumerator Morir()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        empujable = false;
        enemigo.muerto = true;
        Debug.Log("Muriendo");
        animator.Play("Caballero_muerto");
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float duración = animInfo.length;
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }
}
