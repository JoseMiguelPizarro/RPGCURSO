using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventario : MonoBehaviour

{
    public bool InventarioLleno = false;
    public GameObject casilla;

    private List<Item> objetos = new List<Item>();
    public static List<ObjetoInventario> objetosInventario = new List<ObjetoInventario>();
    private List<GameObject> casillas = new List<GameObject>();
    private int CasillaVacia = 0;
    private bool abierto = false;
    public static Inventario inventario;

    private void Awake()
    {
        inventario = this;
    }

    private void Start()
    {
        CargarCasillas();
    }

    private void CargarCasillas()
    {
        foreach (Transform Child in transform)
        {
            casillas.Add(Child.gameObject);
        }
        DeterminarSiguienteCasilla();
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

    public bool AñadirObjeto(Item item)
    {
        DeterminarSiguienteCasilla();
        //Primero checkea si es consumible y además es nuevo objeto
        if (!InventarioLleno)
        {
            if ((item.apilable == true && !objetos.Contains(item) || item.apilable == false))//Revisar si funciona al implementar stack
            {
                Debug.Log("Añadiendo");
                objetos.Add(item);
                GameObject NuevoObjeto = new GameObject();
                NuevoObjeto.AddComponent<ObjetoInventario>();
                NuevoObjeto.AddComponent<Arrastrable>();
                NuevoObjeto.GetComponent<ObjetoInventario>().item = item;
                NuevoObjeto.transform.parent = casillas[CasillaVacia].transform;
                NuevoObjeto.transform.localPosition = Vector2.zero;
                NuevoObjeto.transform.localScale = new Vector3(5, 5, 1); //Ajustar tamaño
                NuevoObjeto.AddComponent<Image>().sprite = item.artwokr;
                NuevoObjeto.name = item.name;
                CasillaVacia++;
                DeterminarSiguienteCasilla();

            }
            else
            {
                Debug.Log("Objeto Apilable");
                for (int i = 0; i < casillas.Count; i++) //Bugeado al eliminar un objeto en una casilla con número superior al número de objetos
                {
                    try
                    {
                        if (item == casillas[i].GetComponentInChildren<ObjetoInventario>().item)
                        {
                            casillas[i].GetComponentInChildren<ObjetoInventario>().CantidadStock++;

                            break;
                        }
                    }
                    catch { }

                }
            }
            PanelInventario.panelInventario.ActualizarTextos();
            return true;
        }
        return false;
    }
    public void UsarObjeto(ObjetoInventario objetoInventario)
    { //Busca Objeto en inventario
        if(objetoInventario.item is Equipamiento)   //Revisa si el objeto es equipable
        {
            Equipamiento objetoEquipado;
            if (PanelEquipamiento.Equipamiento.Equipar((Equipamiento)objetoInventario.item, out objetoEquipado))
            {
                EliminarObjeto(objetoInventario);
                if(objetoEquipado!=null)
                AñadirObjeto(objetoEquipado);
            }
        }
        if (objetoInventario.item.UsarObjeto())
        {
            if (objetoInventario.CantidadStock <= 1)
            {
                objetos.Remove(objetoInventario.item);
            }
            objetoInventario.ReducirStock(1);
        }
    }

    public void EliminarObjeto( ObjetoInventario objetoInventario) //Elimina 1 stock del objeto seleccionado
    {
        objetos.Remove(objetoInventario.item);
        Destroy(objetoInventario.gameObject);
    }

}
