using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.EventSystems;


public class Interactivo : MonoBehaviour,IPointerDownHandler {

    protected ControlJugador player;

    private void Awake()
    {
        Inicializar();
    }

    protected void Interactuar()
    {
        foreach (RaycastHit2D item in player.Interactuar())
        {
            if (item.collider.gameObject == gameObject)
            {
                Interaccion();
            }
        }
            
    }

    protected virtual void Interaccion() //Virtual porque distintos elementos tienen distinta forma de interactuar
    {
        Debug.Log("Interactuó con: " + gameObject.name);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Interactuar();
    }

    protected virtual void Inicializar()
    {
     player = FindObjectOfType<ControlJugador>();
    }
}
