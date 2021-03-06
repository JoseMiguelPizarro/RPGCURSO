﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaballeroAI : EnemigoAI {

    private AnimatorStateInfo animInfo;
    public float blockeoCD = 0.1f;
    private float cd = 0; //Contador para coldown
    // Update is called once per frame
    private void Awake()
    {
        InicializarComponentes();
    }
  
    void FixedUpdate()
    {
       // animInfo = animator.GetCurrentAnimatorStateInfo(0); regular acciones via script
        if (enemigo.muerto == false)
        {
            distanciaJugador = Vector2.Distance(transform.position, jugador.position);
            cd = cd + Time.deltaTime; //Aumentar el colddown del escudo
            if (!atacando && distanciaJugador <= distanciaAtaque) //Atacar
            {
                GenerarDireccion(); //La dirección debe mantenerse mientras se realiza el ataque
                VoltearSprite();
                animator.SetBool("Caminando", false);
                if (blockeoCD <= cd && Random.Range(0, 1) < 0.9)
                {
                    animator.SetTrigger("Defender");
                    cd = 0;
                }
                else 
                {
                    animator.SetTrigger("Atacar");
                }
            }
            else if ((!atacando && (enCombate|| distanciaJugador <= distanciaDetectar)))
            {
                MoverHaciaJugador();
            } //Moverse
            else
            {
                animator.SetBool("Caminando", false);
            }
        }
    }

    private void Dash()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccionAtaque.normalized, 1f);
    }

    public override void RecibirDanio(Transform atacante, int danio)
    {
        if (!enemigo.muerto)
        {
            if (atacable)
            {
                Empujar(atacante);
                miSalud.SaludActual -= danio;
                GenerartextHit(danio.ToString());
                if (enemigo.saludEnemigo.SaludActual <= 0)
                {
                    enemigo.Dropear();
                    StartCoroutine(Morir());
                }
            }
            else
            {
                animator.Play("Caballero_Atacar"); //Contraatque al estar bloqueando
                atacable = true;
            }
        }
    }

    //public override IEnumerator Morir()
    //{
    //    GetComponent<Collider2D>().isTrigger = true;
    //    empujable = false;
    //    enemigo.muerto = true;
    //    Debug.Log("Muriendo");
    //  //  animator.Play("Caballero_muerto");
    //    animator.Play(stateHash,0);
    //    yield return new WaitForSeconds(enemigo.muerteAnim.length);
    //    Destroy(gameObject);
    //}

   public void CaballeroAtacar()
    {
        Atacar();
        Dash();
    }

    public void Blockear()
    {
        atacable = false;
        atacando = true;
        Debug.Log("Bloqueando");
    }

    public void DesBloquear()
    {
        atacable = true;
        atacando = false;
        Debug.Log("Desbloqueando");
    }
}
