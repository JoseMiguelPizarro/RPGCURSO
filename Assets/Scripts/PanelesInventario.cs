using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PanelesInventario : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private bool abierto = false;
    public static PanelesInventario panelesInventario;
    public ToolTip tooltipObjetos;
    public GameObject tooltipEquipo;
    private Vector3 relativePos;
    public UnityEvent OnCerrar;
    public UnityEvent OnAbrir;
    private void Awake()
    {
        panelesInventario = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void AbrirCerrarInventario()
    {
        
        if (abierto)
        {
            OnCerrar?.Invoke();
            Debug.Log("Desactivando Inventario");
            canvasGroup.interactable = false;
            //canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = true;
            abierto = false;
        }
        else
        {
            OnAbrir?.Invoke();
            Debug.Log("Activando Inventario");
            canvasGroup.interactable = true;
            //canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = true;
            abierto = true;
        }

    }

    //public void OnDrag(PointerEventData eventData)
    //{

    //    transform.position = Input.mousePosition - relativePos;
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    relativePos = Input.mousePosition - transform.position;
    //}
}
