using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour {

    public string nombre;
    public int exp;
    public Objeto[] drop;
    public int salud;
    private Rigidbody2D rb;
    private Enemigo enemigo;
    private BarraDeSalud barraDeSalud;

    public int Fuerza = 1;
    public int Velocidad = 1;
    public int Magia = 1;
    public int Inteligencia = 1;

    private int saludActual;
    public int SaludActual {

        get {return saludActual; }

        set { saludActual = value;
            if (saludActual<0)
            {
                saludActual = 0;
            }
        }
         }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemigo = GetComponent<Enemigo>();
        barraDeSalud = GetComponentInChildren<BarraDeSalud>();
        SaludActual = salud;
    }

    public void RecibirDaño(Transform atacante, int daño)
    {
        Debug.Log("dañado " + gameObject.name);
        Vector2 fuerza = (Vector2)transform.position - (Vector2)atacante.position;
        rb.velocity = fuerza.normalized * 20;
        enemigo.SaludActual -= daño;
        barraDeSalud.Actualizar(SaludActual/(float)salud);
        if (SaludActual<=0)
        {
            Morir();
        }
    }
    private void Morir()
    {
        Destroy(gameObject);
    }

    public void Atacar() { }


}
