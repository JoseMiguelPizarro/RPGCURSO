using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasillaEquipamiento : Casilla,IPointerClickHandler {

   [SerializeField] private Equipamiento item;
    private Image image;
    private Color colorNormal = Color.white;
    private Color colorDesactivado = new Color(0, 0, 0, 0);
    public Equipamiento Item { get{ return item; }
        set {
            if (value == null)
            {
                image.color=colorDesactivado;
            }
            else { image.color=colorNormal;
                image.sprite = value.artwork;
            }
            item = value;
        } }
   

    public TipoDeEquipamiento tipoDeEquipamiento;

    private void OnValidate()
    {
        name = tipoDeEquipamiento.ToString();
    }

    private void Start()
    {
        image = GetComponent<Image>();
        if (Item == null)
        {
            image.color= colorDesactivado;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PanelEquipamiento.Equipamiento.DesEquipar(this); 
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (item)
        {
            Debug.Log("Entrando a casilla equipamiento");
            PanelesInventario.panelesInventario.tooltipObjetos.AparecerOcultar(true);
            PanelesInventario.panelesInventario.tooltipObjetos.ActualizarTextos(item.NombreItem, item.StringAtributos());
            PanelesInventario.panelesInventario.tooltipObjetos.transform.position = transform.position;
        }
       
    }
}
