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


    private void Start()
    {
            GetComponent<SpriteRenderer>().sprite = item.artwokr;
    }


    protected override void Interaccion()
    {
        if (Inventario.inventarioSingleton.InventarioLleno == false) //Si se logra ejecutar el Añadir Objeto, corregir y verificar inventario lleno antes y después de añadir objeto
        {
            ObtenerDatos();
            Debug.Log("Interactuó con " + name + " por override");

            Inventario.inventarioSingleton.AñadirObjeto(this);
            Destroy(gameObject);
        }
        else { Debug.Log("Inventario Lleno"); }
    }

    void ObtenerDatos()
    {
        Nombre = item.name;
        Descripción = item.descripcion;
        Sprite = item.artwokr;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("click en " + gameObject);
        Interactuar();  
    }
}
