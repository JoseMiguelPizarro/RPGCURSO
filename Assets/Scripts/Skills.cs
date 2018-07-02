using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour {
    //---Dash---//
    public bool dashAprendido;
    public float dashSpeed = 10;
    public float dashCD = 1;
    private TrailRenderer trail;
    [HideInInspector]
    public bool dashing = false;
    [HideInInspector]
    public bool dashReady = true;
    public float dashDuration = 0.2f;
    //---Bola de fuego---//
    public Proyectil proyectil;
    public float fuerzaRetroceso = 2;
    public string tagObjetivo;
    public float velocidadProyectil;

    
    private void Start()
    {
        if (dashAprendido)
        {
            trail = GetComponent<TrailRenderer>();
        }
    }

    public virtual IEnumerator Dash(Vector2 direccionAtaque)
    {
        if (dashReady)
        {
            dashing = true;
            dashReady = false;
            trail.enabled = true;
            Debug.Log("Dashing");
            for (int i = 0; i < dashDuration*60; i++)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccionAtaque, dashSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            dashing = false;
            yield return new WaitForSeconds(trail.time);
            trail.enabled = false;
            yield return new WaitForSeconds(dashCD);
            dashReady = true;
        }
    }

    public virtual void BolaDeFuego(int inteligencia, Vector2 trayectoria,Rigidbody2D rb)
    {
        Proyectil bolaDeFuego = Instantiate(proyectil, transform.position, Quaternion.identity);
        bolaDeFuego.tagObjetivo = tagObjetivo;
        bolaDeFuego.daño = inteligencia;
        bolaDeFuego.velocidad = velocidadProyectil;
        bolaDeFuego.trayectoria = trayectoria;
        float anguloRotacion = Mathf.Atan2(trayectoria.y, trayectoria.x) * Mathf.Rad2Deg;
        bolaDeFuego.transform.Rotate(0, 0, anguloRotacion);
        rb.velocity = (-trayectoria).normalized * fuerzaRetroceso;
    }
}
