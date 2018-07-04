using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEquipamiento : MonoBehaviour {

    public static PanelEquipamiento Equipamiento;
    

    public CasillaEquipamiento[] casillaEquipamientos;
    public List<Equipamiento> equipamientos = new List<Equipamiento>();

    private void Awake()
    {
        Equipamiento = this;
    }

    private void OnValidate()
    {
        casillaEquipamientos = GetComponentsInChildren<CasillaEquipamiento>();
    }
    private void Start()
    {
        for (int i = 0; i < casillaEquipamientos.Length; i++)
        {

        }
    }

    public bool Equipar(Equipamiento item, out Equipamiento previousItem)
    {
        for (int i = 0; i < casillaEquipamientos.Length; i++)
        {
            if (casillaEquipamientos[i].tipoDeEquipamiento == item.tipoEquipo) //Determinar Casilla
            {
                previousItem = (Equipamiento)casillaEquipamientos[i].Item;
                casillaEquipamientos[i].Item = item;
                equipamientos.Add(item);
                if (previousItem != null)
                {
                    equipamientos.Remove(previousItem);
                }
                ActualizarAtributos();
                if (item.tipoEquipo==TipoDeEquipamiento.Arma)
                {
                    ControlJugador.ArmaEquipado = true;
                }
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    private void ActualizarAtributos()
    {
        AtributosJugador.atributosJugador.ActualizarAtributos(equipamientos);
        PanelEstado.panelEstado.ActualizarTextos();
    }

    public void DesEquipar(CasillaEquipamiento equipamiento)
    {
        if (Inventario.inventario.AñadirObjeto(equipamiento.Item))
        {
            equipamientos.Remove((Equipamiento)equipamiento.Item);
            //if (equipamiento.Item.tip==TipoDeEquipamiento.Arma)
            //{

            //}
            equipamiento.Item = null;

        }
        ActualizarAtributos();
    }
}
