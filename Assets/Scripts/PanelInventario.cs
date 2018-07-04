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
        //AbrirCerrarInventario();
    }
   
    public void UsarObjeto()
    {
        if (objetoSeleccionado)
        {
           inventario.UsarObjeto(objetoSeleccionado);
            ActualizarTextos();
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
