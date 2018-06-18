using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ObjetoInventario : MonoBehaviour,IPointerClickHandler,IDragHandler {

    private Casilla casilla;
    public Item item;
    private int cantidadStock = 1;
    public int CantidadStock {
        get {
            return cantidadStock;
             }
        set {
            cantidadStock =value;
            casilla.ActualizarTextoStock(cantidadStock);
            }
    }

    private void Start()
    {
        casilla = GetComponentInParent<Casilla>();
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        PanelInventario.panelInventario.objetoSeleccionado = this;
        PanelInventario.panelInventario.ActualizarTextos();
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            Inventario.inventario.UsarObjeto(this);
        }
    }
    public void ReducirStock(int cantidadAReducir)
    {
        CantidadStock = CantidadStock - cantidadAReducir <= 0 ? 0: (CantidadStock - cantidadAReducir);
        casilla.ActualizarTextoStock(CantidadStock);
        if (CantidadStock <= 0) {
           Destroy(gameObject);
           PanelInventario.panelInventario.objetoSeleccionado=null;
        }
        else { PanelInventario.panelInventario.ActualizarTextos(); }
    }
    public void OnDestroy()
    {
        casilla.ActualizarTextoStock(0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void Destruir()
    {
        Debug.Log("Objeto destruido es "+gameObject);
        Destroy(gameObject,0f);
    }
}
