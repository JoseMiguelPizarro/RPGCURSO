using System.Collections;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Atacable : MonoBehaviour {
    public bool empujable = true;
    public bool atacable = true;
    [SerializeField] Texto textoHit;


    public virtual void RecibirDanio(Transform atacante, int daño)
    {
        Debug.Log(gameObject.name+" atacado");
        Empujar(atacante);
       // GenerartextHit(daño.ToString());
    }

    protected void Empujar(Transform atacante)
    {
        if (empujable == true)
        {
            Vector2 fuerza = (Vector2)transform.position - (Vector2)atacante.position;
            GetComponent<Rigidbody2D>().velocity = fuerza.normalized * 20;
        }
    }

    protected TextoHit GenerartextHit(string texto)
    {
        Debug.Log("TextHit generado");
      return  textoHit.CrearTextoHit(texto,transform,0.2f,Color.white,new Vector2(-0.5f,0.5f),new Vector2(0,0.5f),1f);
    }

    protected TextoHit GenerartextHit(string texto, float duracion, Color color, float tamaño,Vector2 desfaseX, Vector2 desfaseY)
    {
        Debug.Log("TextHit generado 2");
       return textoHit.CrearTextoHit(texto, transform, tamaño, color, desfaseX, desfaseY, duracion);
    }




    public virtual IEnumerator Morir() { yield return null; }

}
