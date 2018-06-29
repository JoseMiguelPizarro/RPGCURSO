using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextoHit : MonoBehaviour {
    public float tiempoDeVida = 1f;
    public string sortingLayer;
    public float distanciaElevacion = 2;
    private float distanciaActual = 0;
    
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sortingLayerName = sortingLayer;
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        if (distanciaActual<=distanciaElevacion)
        {
            Debug.Log("Subiendo");
            transform.localPosition += new Vector3(0, 0.1f, 0);
            distanciaActual += 0.1f;
        }
    }
}
