using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

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
          TextMesh texthit=  Instantiate(textHit, transform.position, Quaternion.identity,transform);
          textHit.text = texto;
        }
    }

    public virtual IEnumerator Morir() { yield return null; }

}
