using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextoFlotante
{
    public class Texto : MonoBehaviour
    {


        public void CrearTextoFlotante(string contenido, Transform parent, float tamaño,Color color,float desfaseX, float desfaseY,float tiempoDeVida)
        {
            System.Type[] tipos = { typeof(TextMesh),typeof(TextoHit)};
            GameObject textoFlotante=new GameObject("texto",tipos);
            TextMesh textoMesh = textoFlotante.GetComponent<TextMesh>();
            TextoHit textHit = textoFlotante.GetComponent<TextoHit>();
            textoMesh.characterSize = tamaño;
            textoMesh.color = color;
            textoMesh.text = contenido;
            Vector2 desfase = new Vector2(Random.Range( -desfaseX,desfaseX), Random.Range(0,desfaseY));
            Instantiate(textoFlotante, parent.transform.position+(Vector3)desfase,Quaternion.identity);
        }

}
}