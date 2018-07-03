using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour {

    public Vector2 trayectoria;
    public float velocidad;
    public int daño;
    public GameObject explosion;
    public ParticleSystem humito;
    public string tagObjetivo;
    public float tiempoDeVida = 3f;

   
    

    private void FixedUpdate()
    {
        transform.position += (Vector3)trayectoria.normalized * velocidad * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bola de fuego colisionando");
        if (collision.gameObject.tag == tagObjetivo)
        {
            collision.gameObject.GetComponent<Atacable>().RecibirDanio(transform, daño);
            Instantiate(explosion, transform.position, Quaternion.identity);
            humito.transform.parent = null;
            Destroy(humito.gameObject, 5);
                
            Destroy(gameObject);

            //Referencia a la variable
            var emission = humito.emission;
            emission.enabled = false;
        }
        if (collision.gameObject.tag=="muralla")
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            humito.transform.parent = null;
            Destroy(humito.gameObject, 5);
            Destroy(gameObject);
            //Referencia a la variable
            var emission = humito.emission;
            emission.enabled = false;
        }
    }

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}
