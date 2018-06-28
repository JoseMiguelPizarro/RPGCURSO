using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextoHit : MonoBehaviour {
    public float tiempoDeVida = 3f;
    public string sortingLayer;
	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sortingLayerName = sortingLayer;
        Destroy(gameObject, tiempoDeVida);
    }
}
