using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CasillaEquipamiento : MonoBehaviour,IPointerClickHandler {

   [SerializeField] private Item item;
    private Image image;
    public Item Item { get{ return item; }
        set {
            if (value == null)
            {
                image.enabled = false;
            }
            else { image.enabled = true;
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
            image.enabled = false;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PanelEquipamiento.Equipamiento.DesEquipar(this); 
    }
}
