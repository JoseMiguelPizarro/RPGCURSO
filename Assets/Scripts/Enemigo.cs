using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour {

    public string nombre;
    public int exp;
    public Objeto[] drop;
    public int salud;
    private Enemigo enemigo;
    private Animator animator;
    private SpriteRenderer sprite;

    public GestorDeSalud saludEnemigo;
    public AnimationClip muerteAnim;
    public  bool muerto = false;
    public int Fuerza = 1;
    public int Velocidad = 1;
    public int Magia = 1;
    public int Inteligencia = 1;
    

    private void Start()
    {
        enemigo = GetComponent<Enemigo>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        saludEnemigo = GetComponent<GestorDeSalud>();
    }
}
