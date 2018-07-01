using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotonAumentar : MonoBehaviour {

    private int puntos;
    public int Puntos {
        get {return puntos; }
        set {
            {
               puntos=value ;
                Aparecer(puntos);
            }
        } }

    public void Aparecer(int puntoAtributo)
    {
        Debug.Log("Ejecutando evento");
        if (puntoAtributo<=0)
        {
            Debug.Log("Atributos es 0");
            GetComponent<Button>().enabled = false;
            GetComponent<Image>().enabled = false;
        }
        else
        {
            Debug.Log("Atributos mayor que 0");
            GetComponent<Button>().enabled = true;
            GetComponent<Image>().enabled = true;
        }
    }
}
