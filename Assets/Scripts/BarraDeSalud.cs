﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraDeSalud : MonoBehaviour {

    public SpriteRenderer fondo;
    bool atacado = false;
    private Color colorFondo;
    private Color colorBarra; //Color inicial de la barra
    private SpriteRenderer barra;

    public void Start()
    {
        barra = GetComponent<SpriteRenderer>();
        colorFondo = fondo.color;
        colorBarra = barra.color;
        fondo.color = new Color(1, 1, 1, 0);
        barra.color = new Color(1, 1, 1, 0);
    }
    public void Actualizar(float porcentaje)
    {
        if (atacado==false)
        {
            fondo.color = colorFondo;
            barra.color = colorBarra;
        }
        transform.localScale = new Vector3(porcentaje, transform.localScale.y);
    }

 
}