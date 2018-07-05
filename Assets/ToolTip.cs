using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {

    public Text nombreObjeto;
    public Text descripcionObjeto;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ActualizarTextos(string nombre, string descripcion)
    {
        nombreObjeto.text = nombre;
        descripcionObjeto.text = descripcion;
    }
    public void AparecerOcultar(bool abrir)
    {
     
            if (!abrir)
            {
                Debug.Log("Desactivando tooltip");
               // canvasGroup.interactable = false;
                canvasGroup.alpha = 0;
                canvasGroup.ignoreParentGroups = true;
                abrir = false;
            }
            else
            {
                Debug.Log("Activando tooltip");
               // canvasGroup.interactable = true;
                canvasGroup.alpha = 1;
                canvasGroup.ignoreParentGroups = true;
                abrir = true;
            }
    }

}
