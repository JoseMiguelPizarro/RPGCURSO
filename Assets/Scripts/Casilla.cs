using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class Casilla : MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler,IDropHandler {

    private Text txtStock;
    public ObjetoInventario objetoInventario;

    public static GameObject objetoArrastrado; //objeto a arrastrar
    public static GameObject icono; //Icono objeto arrastrado
    public static Casilla casillaPadre; //Casilla donde se encuentra el objeto arrastrado;
    public delegate void DragEvent(GameObject item);
    public static event DragEvent OnItemDragStartEvent;
    public static event DragEvent OnItemDragEndEvent;

    public event Action<Casilla> OnBeginDragEvent;
    public event Action<Casilla> OnEndDragEvent;
    public event Action<Casilla> OnDragEvent;
    public event Action<Casilla> OnDropEvent;

    private Color normalColor = Color.white;
    private Color disableColor = new Color(1, 1, 1, 0);


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

    private void ResetConditions()
    {
        if (icono != null)
        {
            Destroy(icono);                                                          // Destroy icon on item drop
        }
        OnItemDragEndEvent?.Invoke(gameObject);                                         // Notify all cells about item drag end
        objetoArrastrado = null;
        icono = null;
        casillaPadre = null;
        MakeRaycast(true);
    }

    public void MakeRaycast(bool condition)
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = condition;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropeando");
        OnDropEvent?.Invoke(this);
    }

}
