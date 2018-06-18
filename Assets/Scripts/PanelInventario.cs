using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInventario : MonoBehaviour {

    [SerializeField] Inventario inventario;
    public Text txtNombreItem;
    public Text txtDescripcionItem;
    public Text txtCantidadItem;
    public ObjetoInventario objetoSeleccionado;
    public int casillaSeleccionada;
    private CanvasGroup canvasGroup;
    private bool abierto = true;
    static public PanelInventario panelInventario;
    private void Awake()
    {
        panelInventario = this;
        canvasGroup = GetComponent<CanvasGroup>();
        AbrirCerrarInventario();
    }
   
    public void UsarObjeto()
    {
        if (objetoSeleccionado)
        {
           inventario.UsarObjeto(objetoSeleccionado);
            ActualizarTextos();
            //if(objetoSeleccionado.CantidadStock>0)
            //ActualizarTextos();
            //else
            //{
            //   panelInventario.txtNombreItem.text = "";
            //   panelInventario.txtCantidadItem.text = "";
            //   panelInventario.txtDescripcionItem.text = "";
            //}
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

    public void AbrirCerrarInventario()
    {
        if (abierto)
        {
            Debug.Log("Desactivando Inventario");
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
            abierto = false;
        }
        else
        {
            Debug.Log("Activando Inventario");
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;
            abierto = true;
        }
        
    }
    public void Eliminar()
    {
        try
        {
            inventario.EliminarObjeto(objetoSeleccionado);
            ActualizarTextos();
        }
        catch
        {
            ActualizarTextos();
        }
        }
    }
