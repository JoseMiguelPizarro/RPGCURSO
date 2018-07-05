using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {

    public Text nombreObjeto;
    public Text descripcionObjeto;
    private CanvasGroup canvasGroup;
    private bool abierto = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ActualizarTextos(string nombre, string descripcion)
    {
        nombreObjeto.text = nombre;
        descripcionObjeto.text = descripcion;
    }
    public void AparecerOcultar()
    {
     
            if (abierto)
            {
                Debug.Log("Desactivando Inventario");
                canvasGroup.interactable = false;
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.ignoreParentGroups = true;
                abierto = false;
            }
            else
            {
                Debug.Log("Activando Inventario");
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.ignoreParentGroups = true;
                abierto = true;
            }
    }

}
