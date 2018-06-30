﻿using System.Collections;
using UnityEngine;
using TextoFlotante;

[RequireComponent(typeof(Rigidbody2D))]
public class Atacable : MonoBehaviour {
    public bool empujable = true;
    public bool atacable = true;
    [SerializeField] TextoHit textoHit;
    private Texto textoFlotante;
   
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
        textoFlotante.CrearTextoFlotante("Hola",transform,0.3f,Color.blue,0.5f,0.5f,2f);
        Debug.Log("Generando texto hit");
        if (textoHit!=null)
        {
            float desfaseY = Random.Range(0.5f, 1f);
            float desfaseX = Random.Range(-0.5f, 0.5f);
            Vector3 desfase = new Vector3(desfaseX, desfaseY);
            TextoHit hitText=  Instantiate(textoHit,transform.position,Quaternion.identity,transform);
            hitText.transform.localPosition += desfase;
            hitText.textMesh.text = texto;
        }
    }

    protected void GenerartextHit(string texto, float duracion, Color color, float tamaño)
    {
        Debug.Log("Generando texto hit");
        if (textoHit != null)
        {
            float desfaseY = Random.Range(0.5f, 1f);
            float desfaseX = Random.Range(-0.8f, 0.8f);
            Vector3 desfase = new Vector3(desfaseX, desfaseY);
            TextoHit hitText = Instantiate(textoHit, transform.position, Quaternion.identity, transform);
            hitText.transform.localPosition += desfase;
            hitText.textMesh.text = texto;
            hitText.textMesh.color = color;
            hitText.textMesh.characterSize = tamaño;
            hitText.tiempoDeVida = duracion;
        }
    }

    public virtual IEnumerator Morir() { yield return null; }

}
