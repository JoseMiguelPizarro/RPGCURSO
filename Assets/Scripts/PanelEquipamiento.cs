using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEquipamiento : MonoBehaviour {

    public static PanelEquipamiento Equipamiento;
    

    [SerializeField] CasillaEquipamiento[] casillaEquipamientos;
    public List<Equipamiento> equipamientos = new List<Equipamiento>();

    private void Awake()
    {
        Equipamiento = this;
    }

    private void OnValidate()
    {
        casillaEquipamientos = GetComponentsInChildren<CasillaEquipamiento>();
    }

    public bool Equipar(Equipamiento item, out Equipamiento previousItem)
    {
        for (int i = 0; i < casillaEquipamientos.Length; i++)
        {
            if (casillaEquipamientos[i].tipoDeEquipamiento == item.tipoEquipo) //Determinar Casilla
            {
                previousItem = (Equipamiento)casillaEquipamientos[i].Item;
                casillaEquipamientos[i].Item = item;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public void DesEquipar(CasillaEquipamiento equipamiento)
    {
        if (Inventario.inventario.AñadirObjeto(equipamiento.Item))
        {
            equipamiento.Item = null;
        }
    }
}
