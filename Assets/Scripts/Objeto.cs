using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Objeto : Interactivo,IPointerDownHandler
{
    public Item item;
    public string Nombre { get; set; }
    public string Descripción { get; set; }
    public Sprite Sprite { get; set; }
    public bool Consumible { get; set; }

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = item.artwork; 
    }


    private void Start()
    {
            GetComponent<SpriteRenderer>().sprite = item.artwork;
    }


    protected override void Interaccion()
    {
        Debug.Log("Interactuando con " + gameObject.name);
        if (Inventario.inventario.AñadirObjeto(item)) //Si se logra ejecutar el Añadir Objeto, corregir y verificar inventario lleno antes y después de añadir objeto
        {
            Destroy(gameObject);
            Debug.Log("Interactuó con " + name + " por override");
        }
        else { Debug.Log("Inventario Lleno"); }
    }

    void ObtenerDatos()
    {
        Nombre = item.name;
        Descripción = item.descripcion;
        Sprite = item.artwork;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Interactuar();  
    }
}
