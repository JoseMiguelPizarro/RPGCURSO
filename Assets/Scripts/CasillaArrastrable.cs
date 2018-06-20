using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasillaArrastrable : MonoBehaviour,IDropHandler {

    public Arrastrable itemArrastrable;

    public void Start()
    {
        ObtenerItem();
    }

    public void ObtenerItem()
    {
        itemArrastrable = GetComponentInChildren<Arrastrable>();
    }

    private void OnAnyItemDragStart(GameObject item)
    {
        if (itemArrastrable != null)
        {
           itemArrastrable.MakeRaycast(false);
        }
    }

    private void OnAnyItemDragEnd(GameObject item)
    {
        if (itemArrastrable != null)
        {
            itemArrastrable.MakeRaycast(true);                                    // Enable item's raycast
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ObtenerItem();
        Debug.Log(eventData.pointerDrag);
        Debug.Log("En casilla " + name);
        if (eventData!=null)
        {
            
        }
    }
}
