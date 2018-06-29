using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Atacable : MonoBehaviour {
    public bool empujable = true;
    public bool atacable = true;
    [SerializeField] TextMesh textHit;
    
   
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

    protected void GenerartextHit(string texto)
    {
        Debug.Log("Generando texto hit");
        if (textHit!=null)
        {
            float desfaseY = Random.Range(0.5f, 1f);
            float desfaseX = Random.Range(-0.5f, 0.5f);
            Vector3 desfase = new Vector3(desfaseX, desfaseY);
          TextMesh textohit=  Instantiate(textHit,transform.position,Quaternion.identity,transform);
            textohit.transform.localPosition += desfase;
            textohit.text = texto;
        }
    }

    protected void GenerartextHit(string texto, float duracion, Color color)
    {
        Debug.Log("Generando texto hit");
        if (textHit != null)
        {
            float desfaseY = Random.Range(0.5f, 1f);
            float desfaseX = Random.Range(-0.5f, 0.5f);
            Vector3 desfase = new Vector3(desfaseX, desfaseY);
            TextMesh textohit = Instantiate(textHit, transform.position, Quaternion.identity, transform);
            textohit.transform.localPosition += desfase;
            textohit.text = texto;
        }
    }

    public virtual IEnumerator Morir() { yield return null; }

}
