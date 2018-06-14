using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Interactivo : MonoBehaviour {

    public float radio = 5f;
    private ControlJugador player;

    private void Awake()
    {
        player = FindObjectOfType<ControlJugador>();
    }

    protected void Interactuar()
    {
        if (CrossPlatformInputManager.GetButtonDown("Interactuar"))
        {
            if (player.Interactuar() == gameObject)
            {
                Interaccion();
            }
        }
    }
    protected virtual void Interaccion() //Virtual porque distintos elementos tienen distinta forma de interactuar
    {
        Debug.Log("Interactuó con: " + gameObject.name);
        Destroy(gameObject);
    }

}
