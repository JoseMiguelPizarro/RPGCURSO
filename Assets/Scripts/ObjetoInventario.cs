using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjetoInventario : MonoBehaviour,IPointerClickHandler {

    public Item item;
    public int CantidadStock { get; set; }
  

   
    public ObjetoInventario()
    {
        CantidadStock = 1;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Inventario.inventarioSingleton.UsarObjeto(this);
    }
    public void ReducirStock(int cantidadAReducir)
    {
        CantidadStock = CantidadStock - cantidadAReducir <= 0 ? 0: (CantidadStock - cantidadAReducir);
        if (CantidadStock == 0) Destroy(gameObject);
        Debug.Log("El stock es " + CantidadStock);
    }
}
