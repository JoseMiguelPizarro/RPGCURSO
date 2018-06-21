using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Arrastrable : MonoBehaviour/*,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler*/ {

    //public static GameObject objetoArrastrado; //objeto a arrastrar
    //public static GameObject icono; //Icono objeto arrastrado
    //public static Casilla casillaPadre; //Casilla donde se encuentra el objeto arrastrado;
    //public delegate void DragEvent(GameObject item);
    //public static event DragEvent OnItemDragStartEvent;
    //public static event DragEvent OnItemDragEndEvent;

    //public event Action<Arrastrable> OnBeginDragEvent;
    //public event Action<Arrastrable> OnEndDragEvent;
    //public event Action<Arrastrable> OnDragEvent;
    //public event Action<Arrastrable> OnDropEvent;

    //private Color normalColor = Color.white;
    //private Color disableColor = new Color(1, 1, 1, 0);



    //private PanelInventario panelInventario = PanelInventario.panelInventario;

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    OnBeginDragEvent?.Invoke(this);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    OnDragEvent?.Invoke(this);
        
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{

    //    OnEndDragEvent?.Invoke(this);
    //}

    //public Casilla CasillaPadre()
    //{
    //    return GetComponentInParent<Casilla>();
    //}

    //private void ResetConditions()
    //{
    //    if (icono != null)
    //    {
    //        Destroy(icono);                                                          // Destroy icon on item drop
    //    }
    //    OnItemDragEndEvent?.Invoke(gameObject);                                         // Notify all cells about item drag end
    //    objetoArrastrado = null;
    //    icono = null;
    //    casillaPadre = null;
    //    MakeRaycast(true);
    //}

    //public void MakeRaycast(bool condition)
    //{
    //    Image image = GetComponent<Image>();
    //    if (image != null)
    //    {
    //        image.raycastTarget = condition;
    //    }
    //}

    //public void OnDrop(PointerEventData eventData)
    //{
    //    OnDropEvent?.Invoke(this);
    //}
}
