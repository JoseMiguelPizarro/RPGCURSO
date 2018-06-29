using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour {

    public Vector2 trayectoria;
    public float velocidad;
    public int daño;
    public GameObject explosion;

    private void FixedUpdate()
    {
        transform.position += (Vector3)trayectoria.normalized * velocidad * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bola de fuego colisionando");
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<AtributosJugador>().RecibirDanio(transform, daño);
        }
        if (collision.gameObject.tag!="Enemigo")
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
    }

    private void Start()
    {
        Destroy(gameObject, 3);
    }
}
