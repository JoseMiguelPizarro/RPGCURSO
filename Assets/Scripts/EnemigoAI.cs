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
    protected Vector2 direccionAtaque;
    protected Enemigo enemigo;
    public Transform jugador;
    public GameObject puff;
    protected SpriteRenderer sprite;
    protected Atacante atacante;
    protected GestorDeSalud miSalud;
    protected int stateHash;
    protected Skills skills;
    protected Rigidbody2D rb;


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
        jugador = FindObjectOfType<AtributosJugador>().transform;
        rb = GetComponent<Rigidbody2D>();
        skills = GetComponent<Skills>();
    }

    private void Start()
    {
        Instantiate(puff, transform.position,Quaternion.identity,transform);
    }

    protected virtual void MoverHaciaJugador()
    {
        GenerarDireccion();
        enCombate = true;
        animator.SetBool("Caminando", true);
        //transform.position = Vector3.MoveTowards(transform.position, AtributosJugador.atributosJugador.transform.position, 3 * Time.deltaTime);
        transform.position += (Vector3)direccionAtaque.normalized * enemigo.Velocidad * Time.deltaTime;
        VoltearSprite();
    }

    protected virtual Vector2 GenerarDireccion()
    {
        direccionAtaque = jugador.position - transform.position;
        return direccionAtaque;
    }

    protected virtual void VoltearSprite()
    {
        if (direccionAtaque.x < 0)
        {
            sprite.flipX = true;
        }
        else { sprite.flipX = false; }
    }

    public virtual void Atacar()
    {
        atacante.Atacar(direccionAtaque, enemigo.Fuerza);
    }

    public override void RecibirDanio(Transform atacante, int danio)
    {
        if (atacable)
        {
            Empujar(atacante);
            GenerartextHit(danio.ToString());
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
        // GetComponent<Collider2D>().isTrigger = true; //Ignorar colisión con jugador
        // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), jugador.GetComponent<Collider2D>());
        gameObject.layer = 12; // layer Muerto
        empujable = false;
        enemigo.muerto = true;
        Debug.Log("Muriendo");
        //  animator.Play("Caballero_muerto");
        sprite.sortingOrder = 1;
        animator.Play(stateHash, 0);
        GenerartextHit(enemigo.exp.ToString() + " XP",1.3f,Color.green,0.22f,new Vector2(-0.5f,0.5f),new Vector2(0.35f,0.5f));
        AtributosJugador.atributosJugador.Experiencia += enemigo.exp;
        Inventario.inventario.DineroJugador += enemigo.Dinero();
        GenerartextHit(enemigo.Dinero().ToString() + " ORO", 1.3f, Color.yellow, 0.22f, new Vector2(-0.5f, 0.5f), new Vector2(0, 0.3f));
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }

    public virtual void EnemigoComportamiento()
    {
        if (!enemigo.muerto)
        {
            if (!atacando && distanciaJugador < distanciaAtaque) //Atacar
            {
                enCombate = true;
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

    public virtual float CalcularDistanciaJugador()
    {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        return distanciaJugador;
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

    private void InvocarPuff()
    {
        GameObject miPuff = Instantiate(puff, transform.position,Quaternion.identity,null);
    }
}

