using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelesInventario : MonoBehaviour, IDragHandler, IBeginDragHandler
{

    private CanvasGroup canvasGroup;
    private bool abierto = true;
    public static PanelesInventario panelesInventario;
    private Vector3 relativePos;
    private void Awake()
    {
        panelesInventario = this;
        canvasGroup = GetComponent<CanvasGroup>();
        AbrirCerrarInventario();

    }

    public void AbrirCerrarInventario()
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

    public void OnDrag(PointerEventData eventData)
    {

        transform.position = Input.mousePosition - relativePos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        relativePos = Input.mousePosition - transform.position;
    }
}
