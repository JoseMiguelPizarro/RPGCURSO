using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Casilla : MonoBehaviour {

    private Text txtStock;
    private void Awake()
    {
        txtStock = GetComponentInChildren<Text>();
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
