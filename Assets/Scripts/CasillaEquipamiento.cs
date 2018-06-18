using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasillaEquipamiento : MonoBehaviour,IPointerClickHandler {

    private Item item;
    private Image image;
    public Item Item { get{ return item; }
        set {item=value;
            image.sprite = item.artwokr;
            if (item = null)
            {
                image.sprite = null;
            }
        } }

    public TipoDeEquipamiento tipoDeEquipamiento;

    private void OnValidate()
    {
        name = tipoDeEquipamiento.ToString();
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PanelEquipamiento.Equipamiento.DesEquipar(this); 
    }
}
