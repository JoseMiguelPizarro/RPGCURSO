using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        casillaPadre = CasillaPadre();
        objetoArrastrado = gameObject;
        //Crear icono del objeto
        icono = new GameObject();
        icono.transform.SetParent(panelInventario.transform);
        icono.name = "Icon";
        Image myImage = GetComponent<Image>();
        myImage.raycastTarget = false;
        Image iconImage = icono.AddComponent<Image>();
        iconImage.raycastTarget = false;
        iconImage.sprite = myImage.sprite;
        iconImage.color = myImage.color;
        RectTransform iconRect = icono.GetComponent<RectTransform>();
        //Set Icon dimensions
        RectTransform myRect = GetComponent<RectTransform>();
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(myRect.rect.width, myRect.rect.height);

        if (OnItemDragStartEvent != null)
        {
            OnItemDragStartEvent(gameObject);                                   // Notify all items about drag start for raycast disabling
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (icono != null)
        {
            icono.transform.position = Input.mousePosition;                          // Item's icon follows to cursor in screen pixels
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
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
        if (OnItemDragEndEvent != null)
        {
            OnItemDragEndEvent(gameObject);                                         // Notify all cells about item drag end
        }
        objetoArrastrado = null;
        icono = null;
        casillaPadre = null;
    }
}
