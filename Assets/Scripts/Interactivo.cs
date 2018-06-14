using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;


public class Interactivo : MonoBehaviour {

    public float radio = 5f;
    private ControlJugador player;

    private void Awake()
    {
        player = FindObjectOfType<ControlJugador>();
    }

    protected void Interactuar()
    {
            if (player.Interactuar() == gameObject)
            {
               Interaccion();
           }
    }

    protected virtual void Interaccion() //Virtual porque distintos elementos tienen distinta forma de interactuar
    {
        Debug.Log("Interactuó con: " + gameObject.name);
    }

   public void interacciónprueba()
    {
        
    }
}
