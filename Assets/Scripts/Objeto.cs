using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objeto : Interactivo {

    
    public Item item;
    public string Nombre { get; set; }
    public string Descripción { get; set; }
    public Sprite Sprite { get; set; }
    public bool Consumible { get; set; }

    void Update () {
        Interactuar();
	}

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
}
