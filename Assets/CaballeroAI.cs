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
        miSalud = GetComponent<GestorDeSalud>();
        sprite = GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
    }
    void Update()
    {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        animInfo = animator.GetCurrentAnimatorStateInfo(0);
      
        if (enemigo.muerto == false)
        {
            if (distanciaJugador < 2.5 && !animInfo.IsTag("Ataque")) //Atacar
            {
                GenerarDireccion(); //La dirección debe mantenerse mientras se realiza el ataque
                VoltearSprite();
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
            else if ((!animInfo.IsTag("Ataque") && (enCombate|| distanciaJugador <= 6)))
            {
                GenerarDireccion();
                enCombate = true;
                animator.SetBool("Caminando", true);
                //transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
                transform.position += (Vector3)direccion.normalized * enemigo.Velocidad * Time.deltaTime;
                VoltearSprite();
            }
            else
            {
                animator.SetBool("Caminando", false);
            }
            cd += Time.deltaTime; //Aumentar el colddown del escudo
        }
    }

    private void GenerarDireccion()
    {
        direccion = jugador.position - transform.position;
    }

    private void VoltearSprite()
    {
        if (direccion.x < 0)
        {
            sprite.flipX = true;
        }
        else { sprite.flipX = false; }
    }

    public void GuardiaAtaque()
    {
        Debug.Log("GuardiaAtacando");
        Dash();
        atacante.Atacar(direccion, enemigo.Fuerza);
    }

    private void Dash()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccion.normalized, 1f);
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
        else
        {
            animator.Play("Caballero_Atacar"); //Contraatque al estar bloqueando
            atacable = true;
        }
    }

    public override IEnumerator Morir()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        empujable = false;
        enemigo.muerto = true;
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float duracion = animInfo.length;
        Debug.Log("Muriendo");
        animator.Play("Caballero_muerto");
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }

    IEnumerator Fade(float duracion)
    {
        float alpha = 1;
        for (;  alpha>= 0; alpha-=duracion*Time.deltaTime)
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
}
