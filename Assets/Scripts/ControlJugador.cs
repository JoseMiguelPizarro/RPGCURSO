using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum Mirada { Arriba,Abajo,Izquierda,Derecha}


public class ControlJugador : MonoBehaviour {
	private float EscalaInicial;
	private float h;
    [HideInInspector]
    public Vector2 mirada = Vector2.down;
	public float distanciaInteración = 0.5f;
	private int mirando; //-1 izquierda +1 derecha
	public int velocidad;
	private Animator animator;
    public Mirada direcciónMirada = Mirada.Abajo;
	private Atacante atacante;
	private Vector2 direccionAtaque;
    private Rigidbody2D rb;
    private UsarSkill usarSkill;
	
	void Awake () {
		EscalaInicial = transform.localScale.x;
		animator = GetComponent<Animator>();
		atacante = GetComponent<Atacante>();
        rb = GetComponent<Rigidbody2D>();
        usarSkill = GetComponent<UsarSkill>();
	}
	void FixedUpdate () {
		h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
        
		Vector2 movimiento = new Vector2(h, v);
		AnimatorStateInfo animatorState= animator.GetCurrentAnimatorStateInfo(0);

		if (CrossPlatformInputManager.GetButton("Horizontal") || CrossPlatformInputManager.GetButton("Vertical"))
		{
			mirando = (int)Mathf.Sign(h);
			mirada = new Vector2(h, v);
			ActualizarXYAnimator();
		}

		if (CrossPlatformInputManager.GetButtonDown("Atacar")&& !animatorState.IsTag("Atacando"))
		{
			DeterminarDirecciónMirada(mirada);
			animator.SetBool("Corriendo", false);
			animator.SetTrigger("Atacando");
			atacante.Atacar(direcciónMirada,AtributosJugador.atributosJugador.Fuerza);
			Debug.Log(direcciónMirada);
		}
        ///----------Skill 1 --------///

       
        if (usarSkill.dashReady &&CrossPlatformInputManager.GetButton("Skill1")&& !animatorState.IsTag("Atacando") && AtributosJugador.atributosJugador.MagiaActual > 0)
		{
            usarSkill.UsarDash(mirada);
		}

        ///----------Skill 2 --------///
        if (CrossPlatformInputManager.GetButtonDown("Skill2"))
        {
            usarSkill.UsarBolaDeFuego(AtributosJugador.atributosJugador.Inteligencia, mirada, rb);
        }
		else if (!usarSkill.dashing && !animatorState.IsTag("Atacando") && movimiento.magnitude>0 )
		{
			VoltearSprite();
			DeterminarDirecciónMirada(mirada);
			animator.SetBool("Corriendo", true);
			transform.Translate(movimiento * velocidad * Time.deltaTime, Space.World);
		}
		else
		{
			animator.SetBool("Corriendo", false);
		}
        //Debug.DrawRay(transform.position,mirada.normalized*(distanciaInteración+ GetComponent<BoxCollider2D>().size.y / 2), Color.red);
		//Debug.DrawLine(transform.position, transform.position + mirada * 0.2f,Color.green);
		if (CrossPlatformInputManager.GetButtonDown("Inventario"))
		{
			Debug.Log("Desactivando Inventario");
            PanelesInventario.panelesInventario.AbrirCerrarInventario();
		}
	}

	private void VoltearSprite()
	{
		//transform.localScale = new Vector3(Mathf.Sign(h) * EscalaInicial, transform.localScale.y, transform.localScale.z);
		if (Mathf.Sign(h)<0)
		{
			GetComponent<SpriteRenderer>().flipX = true;
		}
		else
		{
			GetComponent<SpriteRenderer>().flipX = false;
		}
	}

	private void ActualizarXYAnimator()
	{
		animator.SetFloat("X", mirada.x);
		animator.SetFloat("Y", mirada.y);
	}

	public RaycastHit2D[] Interactuar()
	{
		RaycastHit2D[] circleCasts = Physics2D.CircleCastAll((Vector2)transform.position+mirada.normalized*GetComponent<CapsuleCollider2D>().size.y/4f, GetComponent<CapsuleCollider2D>().size.y/4f, mirada.normalized, distanciaInteración, LayerMask.GetMask("Interactivo"),-10,10);
		if (circleCasts!=null)
		{
			//Debug.Log("Interactuó con "+ circleCasts.collider.gameObject.name);
			return circleCasts;
		}
		else
		{
			return null;
		}
	}

	void DeterminarDirecciónMirada(Vector2 mirada)
	{
		if (Mathf.Abs( mirada.x)>Mathf.Abs(mirada.y))
		{
			if (mirada.x>=0)
			{
				direccionAtaque = Vector2.right;
				direcciónMirada = Mirada.Derecha;
			}
			else
			{
				direccionAtaque = Vector2.left;
				direcciónMirada = Mirada.Izquierda;
			}
		}
		else if (mirada.y>=0)
		{
			direcciónMirada = Mirada.Arriba;
			direccionAtaque = Vector2.up;
		}
		else
		{
			direcciónMirada = Mirada.Abajo;
			direccionAtaque = Vector2.down;
		}
	   
	}
}
