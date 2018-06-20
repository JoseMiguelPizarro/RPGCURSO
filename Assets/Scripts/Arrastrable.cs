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
        iconRect.localScale = new Vector3(2, 2, 1f);

        OnItemDragStartEvent?.Invoke(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = transform.parent.position;
        MakeRaycast(true);
       
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
}
