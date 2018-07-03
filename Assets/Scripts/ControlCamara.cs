using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlCamara : MonoBehaviour {
    public float zoomSize = 5;
    public float zoomTime = 0.5f;
    private CinemachineVirtualCamera cv;
    private float zoomInicial;
    private bool zooming = false;
    private float tiempoUltimoGolpe = 0;
    private float zoomActual;
    private float zoomPorSegundo;
    private IEnumerator zoomCoroutine=null;
    private void Awake()
    {
        cv = GetComponent<CinemachineVirtualCamera>();
        zoomInicial = cv.m_Lens.OrthographicSize;
    }

    public IEnumerator ZoomIn()
    {
        tiempoUltimoGolpe = 0;
         zoomActual = cv.m_Lens.OrthographicSize;
        if (!zooming)
        {
            zooming = true;
            Debug.Log("Zoom!!");
             zoomPorSegundo = (zoomActual - zoomSize) / (zoomTime) * Time.deltaTime;
            for (; zoomActual > zoomSize; zoomActual -= zoomPorSegundo)
            {
                cv.m_Lens.OrthographicSize = zoomActual;
                yield return new WaitForEndOfFrame();
            }
            for (; tiempoUltimoGolpe < zoomTime*2; tiempoUltimoGolpe+=Time.deltaTime) //Se resetea cada vez que se activa el evento
            {
                yield return new WaitForEndOfFrame();
            }
                StartCoroutine(ResetearZoom());

        }
        else
        {
            yield return null;
        }
    }

    public void ComenzarZoom()
    {
         StartCoroutine(ZoomIn());
        //cv.m_Lens.OrthographicSize = 4;
    }

    public IEnumerator ResetearZoom()
    {
        Debug.Log("ZoomOUT!!");
        zoomPorSegundo = (zoomInicial - zoomActual) / (zoomTime) * Time.deltaTime;
            do 
            {
                zoomActual += zoomPorSegundo;
                cv.m_Lens.OrthographicSize = zoomActual;
                yield return new WaitForEndOfFrame();
            } while (Mathf.Abs(zoomInicial - zoomActual) > Mathf.Abs( zoomPorSegundo));
        cv.m_Lens.OrthographicSize = zoomInicial;
    }

    public IEnumerator ZoomTodo(float zoomObjetivo)
    {
        zoomActual = cv.m_Lens.OrthographicSize;
        zoomPorSegundo = (zoomObjetivo - zoomActual) / (zoomTime) * Time.deltaTime;
        while (Mathf.Abs(zoomObjetivo - zoomActual) > Mathf.Abs( zoomPorSegundo))
        {
            zoomActual += zoomPorSegundo;
            cv.m_Lens.OrthographicSize = zoomActual;
            Debug.Log("Zoomtodo corutina");
            yield return new WaitForEndOfFrame();
            
        }
        cv.m_Lens.OrthographicSize = zoomObjetivo;
        Debug.Log("Terminó zoomtodo");
        yield return new WaitForSeconds(zoomTime * 4);
        StartCoroutine(ResetearZoom());
    }

    public void ejecutarZoomTodo(float zoomObjetivo)
    {
        StopAllCoroutines();
        StartCoroutine(ZoomTodo(zoomObjetivo));
        //StartCoroutine(zoomCoroutine);
        
    }
}
