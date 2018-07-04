﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Inventario : MonoBehaviour

{
    public bool InventarioLleno = false;
    public GameObject casilla;
    public static Inventario inventario;
    private int dineroJugador = 0;
    public int DineroJugador { get {return dineroJugador; } set {dineroJugador=value; } }

    private List<Item> objetos = new List<Item>();
    public static List<GameObject> objetosInventario = new List<GameObject>();
    private Casilla[] listaCasillas;
    [SerializeField] private List<Casilla> casillas = new List<Casilla>();
    private int CasillaVacia = 0;

    private Casilla casillaArrastrada;
    private Text StockArrastrado;
    public ObjetoInventario objetoArrastrado;

    private void Awake()
    {
        inventario = this;
        listaCasillas = GetComponentsInChildren<Casilla>();

    }

    private void Start()
    {
        
        CargarCasillas();           //Suscribirse al evento;
        for (int i = 0; i < casillas.Count; i++)
        {
            listaCasillas[i].OnBeginDragEvent += BeginDrag;
            listaCasillas[i].OnEndDragEvent += EndDrag;
            listaCasillas[i].OnDragEvent += Drag;
            listaCasillas[i].OnDropEvent += Drop;
        }
        for (int i = 0; i < PanelEquipamiento.Equipamiento.casillaEquipamientos.Length; i++)
        {
            PanelEquipamiento.Equipamiento.casillaEquipamientos[i].OnDropEvent += Drop;
        }
        Basurero.basurero.OnDropEvent += Drop;
    }

    private void CargarCasillas()
    {
        casillas = GetComponentsInChildren<Casilla>().ToList();
        DeterminarSiguienteCasilla();
    }

    public void DeterminarSiguienteCasilla()
    {
        CasillaVacia = 0;
        foreach (Casilla casilla in casillas)
        {
            if (casilla.gameObject.GetComponentInChildren<ObjetoInventario>())
            {
                CasillaVacia++;
                if (CasillaVacia >= casillas.Count)
                {
                    InventarioLleno = true;
                    Debug.Log("Inventario Lleno");
                }
            }
            else
            {
                InventarioLleno = false;
                break;
            }
        }

        Debug.Log("Casilla Vacia es "+ CasillaVacia);
    }

    public bool AñadirObjeto(Item item,int cantidad)
    {
        DeterminarSiguienteCasilla();
        //Primero checkea si es consumible y además es nuevo objeto
       
            if (((item.apilable == true && !objetos.Contains(item) && !InventarioLleno)|| (item.apilable == false && !InventarioLleno)))//Revisar si funciona al implementar stack
            {
            System.Type[] componentes = { typeof(ObjetoInventario), typeof(Image) };
                Debug.Log("Añadiendo "+ item.name);
                objetos.Add(item);
                 GameObject nuevoObjeto = new GameObject(item.name, componentes);
                 ObjetoInventario nuevoObjetoInventario = nuevoObjeto.GetComponent<ObjetoInventario>();
                nuevoObjeto.GetComponent<Image>().sprite = item.artwork;
                nuevoObjetoInventario.item = item;
                nuevoObjetoInventario.transform.SetParent(casillas[CasillaVacia].transform);
                nuevoObjetoInventario.transform.localPosition = Vector2.zero;
                nuevoObjetoInventario.transform.localScale = new Vector3(8, 8, 1); //Ajustar tamaño
                nuevoObjetoInventario.name = item.name;
                casillas[CasillaVacia].ObtenerObjetoInventario();
                objetosInventario.Add(nuevoObjeto);
                nuevoObjetoInventario.CantidadStock = cantidad;
            return true;

            }
            else if(item.apilable==true && objetos.Contains(item))
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
            PanelInventario.panelInventario.ActualizarTextos();
            return true;
            }
            return false;
    }

    //Añadir objeto sin especificar cantidad
    public bool AñadirObjeto(Item item)
    {
        return AñadirObjeto(item, 1);  
    }

        public void UsarObjeto(ObjetoInventario objetoInventario)
    { //Busca Objeto en inventario
        if(objetoInventario.item is Equipamiento)   //Revisa si el objeto es equipable
        {
            EquiparObjetoDesdeInventario(objetoInventario);
        }
        else if (objetoInventario.item.UsarObjeto())
        {
            if (objetoInventario.CantidadStock <= 1)
            {
                objetos.Remove(objetoInventario.item);
            }
            objetoInventario.ReducirStock(1);
        }
    }

    private void EquiparObjetoDesdeInventario(ObjetoInventario objetoInventario)
    {
        Equipamiento objetoEquipado;
        if (PanelEquipamiento.Equipamiento.Equipar((Equipamiento)objetoInventario.item, out objetoEquipado))
        {
            Debug.Log("Objeto previamente equipado es " + objetoEquipado);
            DeterminarSiguienteCasilla();
            if (objetoEquipado != null)
            {

                objetoInventario.item = objetoEquipado; //Traspasar Objeto del inventario
                objetoInventario.gameObject.GetComponent<Image>().sprite = objetoEquipado.artwork; //Actualizar artwork del objeto
            }
            else
            {
                objetoInventario.Destruir();
            }
        }
    }

    public void EliminarObjeto( ObjetoInventario objetoInventario) //Elimina 1 stock del objeto seleccionado
    {
        objetos.Remove(objetoInventario.item);
        //objetosInventario.Remove(objetoInventario.gameObject);
        objetoInventario.Destruir();
        Debug.Log("ObjetoInventario eliminado: " + objetoInventario);
        DeterminarSiguienteCasilla();
    }


    //Eventos
    private void BeginDrag(Casilla casilla)
    {
        Debug.Log("Draggeando " + casilla);
        if (casilla.GetComponentInChildren<ObjetoInventario>()!=null)
        {
            casillaArrastrada = casilla;
            objetoArrastrado = casilla.GetComponentInChildren<ObjetoInventario>();
            objetoArrastrado.gameObject.GetComponent<Image>().raycastTarget = false;
            objetoArrastrado.transform.SetParent(PanelInventario.panelInventario.transform);
            
        }
    }

    private void EndDrag(Casilla casilla)
    {
        Debug.Log("Terminó de dragear " + casilla.name);
        if (objetoArrastrado.transform.parent == PanelInventario.panelInventario.transform) //No se soltó en ninguna casilla
        {
            BotarObjeto();
            Instantiate(Resources.Load("Objetos/Prefab/" + objetoArrastrado.item.name), null);
           // Destroy(objetoArrastrado.gameObject);
        }
    }
   

    private void Drag(Casilla casilla)
    {
        if (objetoArrastrado != null)
        {
            objetoArrastrado.transform.position= Input.mousePosition;
            
        }
    }

    private void Drop(Casilla casilla)
    {
        Debug.Log("Dropeando en casilla " + casilla.name);
        ObjetoInventario objetoEnNuevaCasilla = casilla.GetComponentInChildren<ObjetoInventario>();
        if (objetoEnNuevaCasilla!=null) //Existe un objeto en la casilla de destino
        {
            objetoEnNuevaCasilla.transform.position = casillaArrastrada.transform.position;
            objetoEnNuevaCasilla.transform.SetParent(casillaArrastrada.transform);
            objetoEnNuevaCasilla.ActualizarCasillaPadre();
            casillaArrastrada.ActualizarTextoStock(objetoEnNuevaCasilla.CantidadStock);
        }
        else
        { //No existe objeto en la casilla
            casillaArrastrada.ActualizarTextoStock(0);
        }
        if (casilla as CasillaEquipamiento)
        {  //Dropeo objeto en casilla de equipamiento
            Debug.Log("En casilla Equipamiento");
            EquiparObjetoDesdeInventario(objetoArrastrado);
        }
        else if (casilla as Basurero)
        { //Dropeo objeto en basurero
            EliminarObjeto(objetoArrastrado);
            casillaArrastrada.ActualizarTextoStock(0);
        }
        else
        { 
            objetoArrastrado.transform.SetParent(casilla.transform);
            objetoArrastrado.gameObject.GetComponent<Image>().raycastTarget = true;
            objetoArrastrado.transform.position = casilla.transform.position;
            objetoArrastrado.ActualizarCasillaPadre();
            casilla.ActualizarTextoStock(objetoArrastrado.CantidadStock);
        }
    }

    public void BotarObjeto()
    {
        Debug.Log("Objeto dropeado");
    }

    private void RegresarObjetoInvAPosicionInicial()
    {
        
            objetoArrastrado.transform.SetParent(casillaArrastrada.transform);
            objetoArrastrado.transform.position = casillaArrastrada.transform.position;
            objetoArrastrado.gameObject.GetComponent<Image>().raycastTarget = true;
    }
}

