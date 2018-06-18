using UnityEngine;
using UnityEngine.EventSystems;

public class Arrastrable : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {

    public static GameObject objetoArrastrado; //objeto a arrastrar
    public static GameObject icono; //Icono objeto arrastrado
    public static Casilla casillaPadre; //Casilla donde se encuentra el objeto arrastrado;
    public delegate void DragEvent(GameObject item);
    public static event DragEvent OnItemDragStartEvent;
    public static event DragEvent OnItemDragEndEvent;


    private PanelInventario panelInventario = PanelInventario.panelInventario;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent = panelInventario.transform;
        transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
