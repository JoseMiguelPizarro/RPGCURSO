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
    private Vector2 mirada;
	public float distanciaInteración = 1;
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

		if (CrossPlatformInputManager.GetButtonDown("Inventario"))
		{
			Debug.Log("Desactivando Inventario");
			PanelInventario.panelInventario.AbrirCerrarInventario();
		}
       
	  
	}

	public GameObject Interactuar()
	{
		RaycastHit2D circlecast = Physics2D.CircleCast(transform.position, GetComponent<BoxCollider2D>().size.y, mirada.normalized, distanciaInteración, LayerMask.GetMask("Interactivo"),-10,10);
		if (circlecast.collider.gameObject)
		{
			Debug.Log("Interactuó con "+ circlecast.collider.gameObject.name);
			return circlecast.collider.gameObject;
		}
		else
		{
			return null;
		}
	}
}
