using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaballeroAI : Atacable {

	public Transform jugador;
    private Animator animator;
    private Atacante atacante;
    private Enemigo enemigo;
    private float distanciaJugador;
    private Vector2 direccion;
    private AnimatorStateInfo animInfo;
    private SpriteRenderer sprite;
    private GestorDeSalud miSalud;
    private bool enCombate = false;
    public float blockeoCD = 1;
    private float cd = 0; //Contador para coldown
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
            if (distanciaJugador < 2) //Atacar
            {
                if (!animInfo.IsTag("Ataque"))
                {
                    int rand = Random.Range(0, 100);
                    animator.SetBool("Caminando", false);
                    if (rand < 10)
                    {
                        animator.SetTrigger("Atacar");
                    }
                    else if (blockeoCD <= cd)
                    {
                        animator.SetTrigger("Defender");
                        cd = 0;
                    }
                    else
                    {
                        animator.SetTrigger("Atacar");
                    }
                }
            }
            else if ((!animInfo.IsTag("Ataque") && (enCombate|| distanciaJugador <= 6)))
            {
                enCombate = true;
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
            cd += Time.deltaTime;
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
        if (atacable)
        {
            Empujar(atacante);
            miSalud.SaludActual -= danio;
            if (enemigo.saludEnemigo.SaludActual <= 0)
            {
                StartCoroutine(Morir());
            }
        }
        else
        {
            animator.Play("Caballero_Atacar"); //Contraatque al estar bloqueando
            StartCoroutine(ContraAtaque());
            atacable = true;
        }
    }

    public override IEnumerator Morir()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        empujable = false;
        enemigo.muerto = true;
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float duración = animInfo.length;
        Debug.Log("Muriendo");
        animator.Play("Caballero_muerto");
        
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }

    IEnumerator Fade(float duración)
    {
        float alpha = 1;
        for (;  alpha>= 0; alpha-=duración*Time.deltaTime)
        {
            sprite.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }

    public void Blockear()
    {
        atacable = false;
        Debug.Log("Bloqueando");

    }

    public void DesBloquear()
    {
        atacable = true;
        Debug.Log("Desbloqueando");
    }

    IEnumerator ContraAtaque()
    {
        Color colorInicial = sprite.color;
        for (int i = 0; i < 30; i++)
        {
            if (i%2==0)
            {
                sprite.color = colorInicial;
            }
            else
            {
                sprite.color = new Color(1, 1, 1, 1);
            }
            yield return new WaitForEndOfFrame();

        }
    }
}
