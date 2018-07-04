using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Objeto : Interactivo
{
    public Item item;
    public string Nombre { get; set; }
    public string Descripción { get; set; }
    public Sprite Sprite { get; set; }
    public bool Consumible { get; set; }
    private SpriteRenderer spriteRenderer;
    public int cantidad;
    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        GetComponent<SpriteRenderer>().sprite = item.artwork; 
    }


    private void Start()
    {
      spriteRenderer=  GetComponent<SpriteRenderer>();
      spriteRenderer.sprite = item.artwork;
      spriteRenderer.sortingLayerName = "Drops";
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
}
