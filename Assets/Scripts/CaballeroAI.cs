using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaballeroAI : EnemigoAI {

    private AnimatorStateInfo animInfo;

    public float blockeoCD = 1;
    private float cd = 0; //Contador para coldown
    // Update is called once per frame
    private void Awake()
    {
        InicializarComponentes();
    }
  
    void Update()
    {
        distanciaJugador = Vector2.Distance(transform.position, jugador.position);
       // animInfo = animator.GetCurrentAnimatorStateInfo(0); regular acciones via script
        if (enemigo.muerto == false)
        {
            if (!atacando && distanciaJugador < distanciaAtaque) //Atacar
            {
               
                GenerarDireccion(); //La dirección debe mantenerse mientras se realiza el ataque
                VoltearSprite();
                int rand = Random.Range(0, 100);
                animator.SetBool("Caminando", false);
                if (rand < 90 && blockeoCD <= cd &&distanciaJugador<2)
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
            }
            else
            {
                animator.SetBool("Caminando", false);
            }
            cd += Time.deltaTime; //Aumentar el colddown del escudo
        }
    }


    private void Dash()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccion.normalized, 1f);
    }

    public override void RecibirDanio(Transform atacante, int danio)
    {
        if (atacable)
        {
            Empujar(atacante);
            miSalud.SaludActual -= danio;
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

    public override IEnumerator Morir()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        empujable = false;
        enemigo.muerto = true;
        AnimatorStateInfo animInfo = animator.GetCurrentAnimatorStateInfo(0);
        float duracion = animInfo.length;
        Debug.Log("Muriendo");
        animator.Play("Caballero_muerto");
        yield return new WaitForSeconds(enemigo.muerteAnim.length);
        Destroy(gameObject);
    }

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

    public void SetAtacandoFalse()
    {
        atacando = false;
        Debug.Log("Atacando falso");
    }

    public void SetAtacandoTrue()
    {
        atacando = true;
        Debug.Log("Atacando true");
    }
}
