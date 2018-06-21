using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasillaEquipamiento : Casilla,IPointerClickHandler {

   [SerializeField] private Item item;
    private Image image;
    private Color colorNormal = Color.white;
    private Color colorDesactivado = new Color(0, 0, 0, 0);
    public Item Item { get{ return item; }
        set {
            if (value == null)
            {
                image.color=colorDesactivado;
            }
            else { image.color=colorNormal;
                image.sprite = value.artwokr;
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
}
