using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Casilla : MonoBehaviour {

    private Text txtStock;
    private void Awake()
    {
        txtStock = GetComponentInChildren<Text>();
    }
    private void Start()
    {

    }

    public void ActualizarTextoStock(int stock)
    {
        if (stock > 1)
        {
            txtStock.text = stock.ToString();
        }
        else txtStock.text = "";
    }

   
}
