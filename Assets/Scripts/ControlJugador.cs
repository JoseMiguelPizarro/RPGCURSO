using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class ControlJugador : MonoBehaviour {
	private AtributosJugador atributos = AtributosJugador.atributosJugador;
	public GameObject inventario;
	private float EscalaInicial;
	private float h;
    private Vector3 mirada;
	public float distanciaInteración = 0.5f;
	private int mirando; //-1 izquierda +1 derecha
	public int velocidad;
    private Animator animator;
	
	void Awake () {
		EscalaInicial = transform.localScale.x;
        animator = GetComponent<Animator>();
	}
	void Update () {
		h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");

		Vector2 movimiento = new Vector2(h, v);
        AnimatorStateInfo animatorState= animator.GetCurrentAnimatorStateInfo(0);
        
        
        if (CrossPlatformInputManager.GetButtonDown("Atacar"))
        {
            animator.SetBool("Corriendo", false);
            animator.SetTrigger("Atacando");
        }
       else if (!animatorState.IsName("Atacando") && movimiento.magnitude>0)
        {
            animator.SetBool("Corriendo", true);
            transform.Translate(movimiento * velocidad * Time.deltaTime, Space.World);
        }
        else
        {
            animator.SetBool("Corriendo", false);
        }

		if (CrossPlatformInputManager.GetButton("Horizontal") || CrossPlatformInputManager.GetButton("Vertical"))
		{
			transform.localScale = new Vector3(Mathf.Sign(h) * EscalaInicial, transform.localScale.y, transform.localScale.z);
			mirando = (int)Mathf.Sign(h);
            mirada = new Vector2(h, v);
		}
       
		Debug.DrawRay(transform.position,mirada.normalized*(distanciaInteración+ GetComponent<BoxCollider2D>().size.y / 2), Color.red);
        Debug.DrawLine(transform.position, transform.position + mirada * 0.2f,Color.green);
		if (CrossPlatformInputManager.GetButtonDown("Inventario"))
		{
			Debug.Log("Desactivando Inventario");
			PanelInventario.panelInventario.AbrirCerrarInventario();
		}
       
	  
	}

	public RaycastHit2D[] Interactuar()
	{
		RaycastHit2D[] circleCasts = Physics2D.CircleCastAll(transform.position+mirada.normalized*GetComponent<BoxCollider2D>().size.y/4f, GetComponent<BoxCollider2D>().size.y/4f, mirada.normalized, distanciaInteración, LayerMask.GetMask("Interactivo"),-10,10);
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
}
