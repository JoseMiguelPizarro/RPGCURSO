using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGuardia : MonoBehaviour {

	public Transform jugador;
    private Animator animator;
    private Rigidbody2D rb;
    private Atacante atacante;
    private Enemigo guardia;
    private float distanciaJugador;
    private Vector2 dirección;
    private AnimatorStateInfo animInfo;
    // Update is called once per frame
    private void Start()
    {
        atacante = GetComponent<Atacante>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animInfo = animator.GetCurrentAnimatorStateInfo(0);
    }
    void Update () {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        dirección = jugador.position - transform.position;
        if (distanciaJugador<2)
        {
            animator.SetBool("Corriendo", false);
            animator.SetTrigger("Atacar");
            atacante.Atacar(dirección,guardia.Fuerza );
        }
        if (distanciaJugador<=10&& animInfo.IsTag("Atacando"))
        {
            animator.SetBool("Caminando", true);
            transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
            if (dirección.x<0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else { GetComponent<SpriteRenderer>().flipX = false; }
        }
    }
}
