using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour {

    public string nombre;
    public int exp;
    private Enemigo enemigo;
    private Animator animator;
    private SpriteRenderer sprite;
    private Drop drop;

    [HideInInspector]
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
        drop = GetComponent<Drop>();
    }

    public void Dropear()
    {
        Debug.Log("Dropeando");
        for (int i = 0; i < drop.drops.Length; i++)
        {
            float rate = Random.Range(0, 100);
            if (rate<=drop.dropRate[i])
            {
                Vector2 desfaseDrop = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                Objeto dropeo= Instantiate(drop.drops[i], transform.position+(Vector3)desfaseDrop,Quaternion.identity);
                dropeo.transform.parent = GameObject.Find("Objetos").transform;
            }
        }
    }

    public int Dinero()
    {
        return drop.dinero;
    }

}
