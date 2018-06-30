using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextoHit : MonoBehaviour {
    public float tiempoDeVida = 1f;
    public string sortingLayer="TEXTO";
    public float distanciaElevacion = 2;
    private float distanciaActual = 0;
    public TextMesh textMesh;
    private float tiempoInicio;
    private bool desvaneciendo=false;
    public Color color;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }
    void Start () {
        tiempoInicio = Time.time;
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
        if (!desvaneciendo && (Time.time-tiempoInicio)>=tiempoDeVida*0.5)
        {
            desvaneciendo = true;
            Debug.Log("Desvaneciendo");
            StartCoroutine(Desvanecer());
        }
    }

   IEnumerator Desvanecer()
    {
        Color colorActual = textMesh.color;
        for (float alpha = 1; alpha >=0; alpha-=(1/(tiempoDeVida*0.5f))*Time.deltaTime)
        {
            colorActual.a = alpha;
            textMesh.color = colorActual;
            yield return new WaitForEndOfFrame();
        }
    }
}
