using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Texto : MonoBehaviour
    {
    public TextoHit textoHit;
        public TextoHit CrearTextoHit(string contenido, Transform parent, float tamaño,Color color,Vector2 desfaseX, Vector2 desfaseY,float tiempoDeVida)
        {
            textoHit.tiempoDeVida = tiempoDeVida;
            Vector3 desfase = new Vector2(Random.Range(desfaseX.x, desfaseX.y), Random.Range(desfaseY.x, desfaseY.y));
            TextoHit texto = Instantiate(textoHit, parent.transform.position + desfase, Quaternion.identity, parent); //Inicializar primero, luego definir parámetros
            texto.textMesh.color = color;
            texto.textMesh.characterSize = tamaño;
            texto.textMesh.text = contenido;
            return texto;
    }
}
