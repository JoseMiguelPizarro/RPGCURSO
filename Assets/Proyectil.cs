using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour {

    public Vector2 trayectoria;
    public float velocidad;
    public int daño;

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
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, 3);
    }
}
