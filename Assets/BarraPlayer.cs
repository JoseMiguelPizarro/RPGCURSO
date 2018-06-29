using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraPlayer : MonoBehaviour {

    RectTransform rectTransform;
    
    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Actualizar(float porcentaje)
    {
        Debug.Log(porcentaje);
        rectTransform.localScale = new Vector3(porcentaje, rectTransform.localScale.y,1);
    }
}
