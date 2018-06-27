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
    private void Awake()
    {
        atacante = GetComponent<Atacante>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        guardia = GetComponent<Enemigo>();
    }
    void Update () {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        dirección = jugador.position - transform.position;
        animInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (distanciaJugador<2)
        {
            animator.SetBool("Caminando", false);
            animator.SetTrigger("Atacar");
        }
        else if (!animInfo.IsTag("Ataque") && distanciaJugador <= 6)
        {
            animator.SetBool("Caminando", true);
            //transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
            transform.position += (Vector3)dirección.normalized * 3 * Time.deltaTime;
            if (dirección.x<0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else { GetComponent<SpriteRenderer>().flipX = false; }
        }
    }
    public void GuardiaAtaque()
    {
        Debug.Log("GuardiaAtacando");
        atacante.Atacar(dirección, guardia.Fuerza);
        transform.position = Vector3.MoveTowards(transform.position, transform.position+ (Vector3)dirección.normalized*3, 0.5f);
    }
}
