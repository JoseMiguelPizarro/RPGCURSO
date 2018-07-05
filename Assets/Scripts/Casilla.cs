using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class Casilla : MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler,IDropHandler,IPointerEnterHandler,IPointerExitHandler {

    private Text txtStock;
    public ObjetoInventario objetoInventario;

    public delegate void DragEvent(Casilla casilla);

    public event DragEvent OnBeginDragEvent;
    public event DragEvent OnEndDragEvent;
    public event DragEvent OnDragEvent;
    public event DragEvent OnDropEvent;
    


    private void Awake()
    {
        txtStock = GetComponentInChildren<Text>();
        ObtenerObjetoInventario();
    }

    public void ObtenerObjetoInventario()
    {
        objetoInventario = GetComponentInChildren<ObjetoInventario>();
    }

    public void ActualizarTextoStock(int stock)
    {
        if (stock > 1)
        {
            txtStock.text = stock.ToString();
        }
        else txtStock.text = "";
    }

    //Sistema de Eventos
    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragEvent?.Invoke(this);
        ObtenerObjetoInventario();
    }

    public Casilla CasillaPadre()
    {
        return GetComponentInParent<Casilla>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropeando");
        OnDropEvent?.Invoke(this);
        ObtenerObjetoInventario();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (objetoInventario.item is Equipamiento)
        {
            PanelesInventario.panelesInventario.tooltipObjetos.AparecerOcultar(true);
            Equipamiento equipo = (Equipamiento)objetoInventario.item;
            PanelesInventario.panelesInventario.tooltipObjetos.ActualizarTextos(objetoInventario.item.NombreItem, equipo.StringAtributos());
            PanelesInventario.panelesInventario.tooltipObjetos.transform.position = transform.position;
        }
        else if (objetoInventario.item) //Tooltip para objetos
        {
            PanelesInventario.panelesInventario.tooltipObjetos.AparecerOcultar(true);
            PanelesInventario.panelesInventario.tooltipObjetos.ActualizarTextos(objetoInventario.item.NombreItem, objetoInventario.item.descripcion);
            PanelesInventario.panelesInventario.tooltipObjetos.transform.position = transform.position;
        }

        if(!objetoInventario)
        {
            PanelesInventario.panelesInventario.tooltipObjetos.AparecerOcultar(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            PanelesInventario.panelesInventario.tooltipObjetos.AparecerOcultar(false);
    }
}
