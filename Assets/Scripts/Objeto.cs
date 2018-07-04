using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof(SpriteRenderer),typeof(BoxCollider2D))]
public class Objeto : Interactivo
{
    public Item item;
    public string Nombre { get; set; }
    public string Descripción { get; set; }
    public Sprite Sprite { get; set; }
    public bool Consumible { get; set; }
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxcollider;
    public int cantidad = 1;
    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.name = item.NombreItem;
        GetComponent<SpriteRenderer>().sprite = item.artwork; 
    }


    private void Start()
    {
        Inicializar();
        boxcollider = GetComponent<BoxCollider2D>();
        boxcollider.isTrigger = true;
        boxcollider.size = new Vector2(1, 1);
        gameObject.name = item.NombreItem;
        spriteRenderer =  GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.artwork;
        spriteRenderer.sortingLayerName = "Drops";
        spriteRenderer.material = GestorMateriales.gestorMateriales.materiales[0];
        gameObject.layer = 9; //Capa interactuable
    }

    protected override void Interaccion()
    {
        Debug.Log("Interactuando con " + gameObject.name);
        if (Inventario.inventario.AñadirObjeto(item,cantidad)) //Si se logra ejecutar el Añadir Objeto, corregir y verificar inventario lleno antes y después de añadir objeto
        {
            Destroy(gameObject);
            Debug.Log("Interactuó con " + name + " por override");
        }
        else { Debug.Log("Inventario Lleno"); }
    }
}
