using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class ControlJugador : MonoBehaviour {
    public float velocidad = 1;
    public GameObject inventario;
    private float EscalaInicial;
    private float h;
    public float distanciaInteración = 1;
    private int mirando; //-1 izquierda +1 derecha
    
    void Awake () {
        EscalaInicial = transform.localScale.x;
    }
	void Update () {
        h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        
        Vector2 movimiento = new Vector2(h, v);
        transform.Translate(movimiento * velocidad * Time.deltaTime,Space.World);

        if (CrossPlatformInputManager.GetButton("Horizontal"))
        {
            transform.localScale = new Vector3(Mathf.Sign(h) * EscalaInicial, transform.localScale.y, transform.localScale.z);
            mirando = (int)Mathf.Sign(h);
        }
        Debug.DrawRay(transform.position, Vector2.right * mirando*(distanciaInteración+ GetComponent<BoxCollider2D>().size.y / 2), Color.red);

        if (CrossPlatformInputManager.GetButtonDown("Inventario"))
        {
            Debug.Log("Desactivando Inventario");
            PanelInventario.panelInventario.AbrirCerrarInventario();
        }
      
    }

    public GameObject Interactuar()
    {
        RaycastHit2D circlecast = Physics2D.CircleCast(transform.position, GetComponent<BoxCollider2D>().size.y / 2, Vector2.right*mirando, distanciaInteración, LayerMask.GetMask("Interactivo"),-10,10);
        if (circlecast.collider)
        {
            Debug.Log("Interactuó");
            return circlecast.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
}
