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
        PanelInventario.panelInventario.objetoSeleccionado = this;
        PanelInventario.panelInventario.ActualizarTextos();
    }
    public void ReducirStock(int cantidadAReducir)
    {
        CantidadStock = CantidadStock - cantidadAReducir <= 0 ? 0: (CantidadStock - cantidadAReducir);
        if (CantidadStock <= 0) {
           PanelInventario.panelInventario.txtNombreItem.text = "";
            PanelInventario.panelInventario.txtCantidadItem.text = "";
            PanelInventario.panelInventario.txtDescripcionItem.text = "";
            Debug.Log("Destruyendo "+gameObject);
            Destroy(gameObject);
            PanelInventario.panelInventario.objetoSeleccionado=null;
        }
        else { PanelInventario.panelInventario.ActualizarTextos(); }
    }
}
