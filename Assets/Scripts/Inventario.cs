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

    private List<Item> objetos = new List<Item>();
    public static List<GameObject> objetosInventario = new List<GameObject>();
    private Casilla[] listaCasillas;
    [SerializeField] private List<Casilla> casillas = new List<Casilla>();
    private int CasillaVacia = 0;
    private bool abierto = false;

    public event Action<Casilla> OnBeginDragEvent;
    public event Action<Casilla> OnEndDragEvent;
    public event Action<Casilla> OnDragEvent;
    public event Action<Casilla> OnDropEvent;
    private Casilla casillaArrastrada;
    private ObjetoInventario objetoArrastrado;

    private void Awake()
    {
        inventario = this;
        listaCasillas = GetComponentsInChildren<Casilla>();

    }

    private void Start()
    {
        CargarCasillas();
        for (int i = 0; i < casillas.Count; i++)
        {
            listaCasillas[i].OnBeginDragEvent += BeginDrag;
            listaCasillas[i].OnEndDragEvent += EndDrag;
            listaCasillas[i].OnDragEvent += Drag;
            listaCasillas[i].OnDropEvent += Drop;
        }
    }

    private void CargarCasillas()
    {
        casillas = GetComponentsInChildren<Casilla>().ToList();
        DeterminarSiguienteCasilla();
    }

    public void DeterminarSiguienteCasilla()
    {
        CasillaVacia = 0;
        //for (int i = 0; i < casillas.Count; i++)
        //{
        //    if (!casillas[i].GetComponentInChildren<ObjetoInventario>())
        //    {
        //        CasillaVacia = i;
        //        if (CasillaVacia<casillas.Count)
        //        {
        //            InventarioLleno = false;
        //        }
        //    }
        //    else
        //    {
        //        InventarioLleno = true;
        //    }
        //}


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

    public bool AñadirObjeto(Item item)
    {
        DeterminarSiguienteCasilla();
        //Primero checkea si es consumible y además es nuevo objeto
       
            if (((item.apilable == true && !objetos.Contains(item) && !InventarioLleno)|| (item.apilable == false && !InventarioLleno)))//Revisar si funciona al implementar stack
            {
           
                Debug.Log("Añadiendo "+ item.name);
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
                casillas[CasillaVacia].ObtenerObjetoInventario();
                objetosInventario.Add(NuevoObjeto);
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
    public void UsarObjeto(ObjetoInventario objetoInventario)
    { //Busca Objeto en inventario
        if(objetoInventario.item is Equipamiento)   //Revisa si el objeto es equipable
        {
            Equipamiento objetoEquipado;
            if (PanelEquipamiento.Equipamiento.Equipar((Equipamiento)objetoInventario.item, out objetoEquipado))
            {
                Debug.Log("Objeto previamente equipado es "+ objetoEquipado);
                DeterminarSiguienteCasilla();
                if (objetoEquipado != null)
                {
                    objetoInventario.item = objetoEquipado; //Traspasar Objeto del inventario
                    objetoInventario.gameObject.GetComponent<Image>().sprite = objetoEquipado.artwokr; //Actualizar artwork del objeto
                }
                else
                {
                    objetoInventario.Destruir();
                }
            }
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
        Debug.Log("Terminó de dragear");
        if (objetoArrastrado.transform.parent == PanelInventario.panelInventario.transform)
        {
            objetoArrastrado.transform.SetParent(casillaArrastrada.transform);
            objetoArrastrado.transform.position = casillaArrastrada.transform.position;
        }

    }
    private void Drag(Casilla casilla)
    {
        if (objetoArrastrado != null)
        {
            objetoArrastrado.transform.position = Input.mousePosition;
        }
    }

    private void Drop(Casilla casilla)
    {
        Debug.Log("Dropeando en casilla " + casilla.name);
        ObjetoInventario objetoEnNuevaCasilla = casilla.GetComponentInChildren<ObjetoInventario>();
        if (objetoEnNuevaCasilla!=null)
        {
            objetoEnNuevaCasilla.transform.position = casillaArrastrada.transform.position;
            objetoEnNuevaCasilla.transform.SetParent(casillaArrastrada.transform);

        }
            objetoArrastrado.transform.SetParent(casilla.transform);
            objetoArrastrado.gameObject.GetComponent<Image>().raycastTarget = true;
            objetoArrastrado.transform.position = casilla.transform.position;
        }
    }

