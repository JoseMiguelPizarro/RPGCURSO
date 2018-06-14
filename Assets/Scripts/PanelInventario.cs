using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInventario : MonoBehaviour {

    public Text txtNombreItem;
    public Text txtDescripcionItem;
    public Text txtCantidadItem;
    public ObjetoInventario objetoSeleccionado;

    static public PanelInventario panelInventario;
    private void Awake()
    {
        panelInventario = this;
    }
   
    public void UsarObjeto()
    {
        if (objetoSeleccionado)
        {
            Inventario.inventarioSingleton.UsarObjeto(objetoSeleccionado);
            if(objetoSeleccionado.CantidadStock>0)
            ActualizarTextos();
            else
            {
               panelInventario.txtNombreItem.text = "";
               panelInventario.txtCantidadItem.text = "";
               panelInventario.txtDescripcionItem.text = "";
            }
        }
    }

    public void ActualizarTextos()
    {
        try
        {
            panelInventario.txtNombreItem.text = objetoSeleccionado.item.name;
            if (objetoSeleccionado.item.apilable == true)
                 panelInventario.txtCantidadItem.text = objetoSeleccionado.CantidadStock.ToString();
            else panelInventario.txtCantidadItem.text = "";
            panelInventario.txtDescripcionItem.text = objetoSeleccionado.item.descripcion;
        }
        catch { };
    }

    public void CerrarInventario()
    {
        gameObject.SetActive(false);
    }
    public void Eliminar()
    {
        try
        {
            Inventario.inventarioSingleton.EliminarObjeto(objetoSeleccionado);
        }
        catch { }
    }
}
