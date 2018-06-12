using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventario : MonoBehaviour {

    static public Inventario inventarioSingleton;
    public int TamañoInventario { get; set; }
    private Hashtable nombreObjetos;
    private void Awake()
    {
        inventarioSingleton = this;
        inventarioSingleton.TamañoInventario = 12;
    }

    static List<GameObject> casillas = new List<GameObject>();
     int CasillaVacia = 0;
    public bool InventarioLleno = false;
    private void Start()
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

    public void AñadirObjeto(Objeto objeto)
    {
        Debug.Log("AñadirObjeto activado");
        DeterminarSiguienteCasilla();

      //Primero checkea si es consumible y además es nuevo objeto
        if (objeto.item.Consumible == false || !nombreObjetos.ContainsValue(objeto.name)) //Revisar si funciona al implementar stack
        {
            Debug.Log("Añadiendo");
            GameObject NuevoObjeto = new GameObject();
            NuevoObjeto.AddComponent<ObjetoInventario>();
            NuevoObjeto.GetComponent<ObjetoInventario>().item = objeto.item;
            NuevoObjeto.transform.parent = casillas[CasillaVacia].transform;
            NuevoObjeto.transform.localPosition = Vector2.zero;
            NuevoObjeto.AddComponent<Image>().sprite = objeto.Sprite;
            NuevoObjeto.name = objeto.Nombre;
            CasillaVacia++;
            nombreObjetos.Add(CasillaVacia, objeto);
            DeterminarSiguienteCasilla();
        }
        else
        {
            int casilla = 0;
            for ( casilla = 0; casilla < TamañoInventario; casilla++)
            {
                if (casillas[casilla].GetComponent<Casilla>().objetoInventario.name==nombreObjetos[casilla].ToString() )
                {
                    casillas[casilla].GetComponent<Casilla>().objetoInventario.CantidadStack++;
                    Debug.Log("stack "+casillas[casilla].GetComponent<Casilla>().objetoInventario.CantidadStack);
                    break;
                }
            }
        }
    }

}
