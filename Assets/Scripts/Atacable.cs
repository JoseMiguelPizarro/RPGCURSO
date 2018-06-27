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


    public virtual void RecibirDanio(Transform atacante, int daño)
    {
        Empujar(atacante);
        Debug.Log("dañado " + gameObject.name);


    }

    protected void Empujar(Transform atacante)
    {
        if (empujable == true)
        {
            Vector2 fuerza = (Vector2)transform.position - (Vector2)atacante.position;
            GetComponent<Rigidbody2D>().velocity = fuerza.normalized * 20;
        }
    }

    public virtual IEnumerator Morir() { yield return null; }

}
