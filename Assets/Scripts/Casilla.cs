using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class Casilla : MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler,IDropHandler {

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
    }

    public Casilla CasillaPadre()
    {
        return GetComponentInParent<Casilla>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropeando");
        OnDropEvent?.Invoke(this);
    }

}
