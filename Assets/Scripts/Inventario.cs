using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventario : MonoBehaviour

{
   
    public int TamañoInventario { get; set; }
    public bool InventarioLleno = false;
    static public Inventario inventarioSingleton;
    private Dictionary<int, Item> ObjetosEnInventario = new Dictionary<int, Item>();

    private List<GameObject> casillas = new List<GameObject>();
    private int CasillaVacia = 0;
    private bool Abierto = false;

    private void Awake()
    {
        inventarioSingleton = this;
        inventarioSingleton.TamañoInventario = 12;

    }
    private void Start()
    {
        foreach (Transform Child in transform)
        {
            casillas.Add(Child.gameObject);
        }
        DeterminarSiguienteCasilla();
        PanelInventario.panelInventario.gameObject.SetActive(false); //Una vez cargadas las casillas se oculta el inventario
    }
    private void DeterminarSiguienteCasilla()
    {
        CasillaVacia = 0;
        foreach (GameObject casilla in casillas)
        {
            if (casilla.GetComponentInChildren<ObjetoInventario>())
            {
                CasillaVacia++;
                if (CasillaVacia >= casillas.Count)
                {
                    InventarioLleno = true;
                    
                }
            }
            else break;
        }
        if (CasillaVacia >= casillas.Count)
        {
            InventarioLleno = true;
        }
        Debug.Log(CasillaVacia);
    }

    public void AñadirObjeto(Objeto objeto)
    {
        Debug.Log("AñadirObjeto activado");
        DeterminarSiguienteCasilla();
        
        
      //Primero checkea si es consumible y además es nuevo objeto
        if ((objeto.item.apilable == true && !ObjetosEnInventario.ContainsValue(objeto.item)) || objeto.item.apilable==false) //Revisar si funciona al implementar stack
        {
            Debug.Log("Añadiendo");
            GameObject NuevoObjeto = new GameObject();
            NuevoObjeto.AddComponent<ObjetoInventario>();
            NuevoObjeto.GetComponent<ObjetoInventario>().item = objeto.item;
            NuevoObjeto.transform.parent = casillas[CasillaVacia].transform;
            NuevoObjeto.transform.localPosition = Vector2.zero;
            NuevoObjeto.transform.localScale = new Vector3(4, 4, 1); //Ajustar tamaño
            NuevoObjeto.AddComponent<Image>().sprite = objeto.Sprite;
            NuevoObjeto.name = objeto.Nombre;
            ObjetosEnInventario.Add(CasillaVacia, objeto.item);
            CasillaVacia++;
            DeterminarSiguienteCasilla();
        }
        else
        { //Apila objeto en inventario
            Debug.Log("El objeto es consumible");
            int key = ObjetosEnInventario.Where(pair => pair.Value == objeto.item)
                      .Select(pair => pair.Key)
                      .FirstOrDefault();
            casillas[key].GetComponentInChildren<ObjetoInventario>().CantidadStock++;
            Debug.Log("Objeto añadido a key => " +key);
            Debug.Log("El stock es " + casillas[key].GetComponentInChildren<ObjetoInventario>().CantidadStock);
        }
        PanelInventario.panelInventario.ActualizarTextos();
    }
    public void UsarObjeto(ObjetoInventario objetoInventario)
    { //Busca Objeto en inventario
        EliminarObjeto(objetoInventario);
        objetoInventario.item.UsarObjeto();
    }

    public void EliminarObjeto( ObjetoInventario objetoInventario) //Elimina 1 stock del objeto seleccionado
    {
        int key = ObjetosEnInventario.Where(pair => pair.Value == objetoInventario.item)
                             .Select(pair => pair.Key)
                             .FirstOrDefault();
       

        Debug.Log("La llave a usar es :" + key +"La llave 2 es ");
        
        //Retira Objeto de la pila
        casillas[key].GetComponentInChildren<ObjetoInventario>().ReducirStock(1);
        if (casillas[key].GetComponentInChildren<ObjetoInventario>().CantidadStock <= 0)
        {
            Debug.Log("Eliminando objeto del inventario");
            ObjetosEnInventario.Remove(key);
        }
    }

}
