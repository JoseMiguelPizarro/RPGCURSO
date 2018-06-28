using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour {

    public Vector2 trayectoria;
    public float velocidad;

    private void Update()
    {
        transform.position += (Vector3)trayectoria.normalized * velocidad * Time.deltaTime;
    }

    public IEnumerator Disparar(Vector2 trayectoria, float velocidad, float tiempoDeVida)
    {
        Destroy(gameObject, tiempoDeVida);
        while (true)
        {
            transform.position = transform.position + (Vector3)trayectoria.normalized * velocidad * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
