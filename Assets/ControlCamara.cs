using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ControlCamara : MonoBehaviour {
    public float zoomSize;
    public float zoomTime;
    private CinemachineVirtualCamera cv;
    private float zoomInicial;
    private bool zooming = false;
    private float tiempoUltimoGolpe = 0;
    private void Awake()
    {
        cv = GetComponent<CinemachineVirtualCamera>();
        zoomInicial = cv.m_Lens.OrthographicSize;
    }

    public IEnumerator ZoomIn()
    {
        tiempoUltimoGolpe = 0;
        float zoomActual = cv.m_Lens.OrthographicSize;
        if (!zooming)
        {
            zooming = true;
            Debug.Log("Zoom!!");
            float zoomPorSegundo = (zoomActual - zoomSize) / (zoomTime) * Time.deltaTime;
            for (; zoomActual > zoomSize; zoomActual -= zoomPorSegundo)
            {
                cv.m_Lens.OrthographicSize = zoomActual;
                yield return new WaitForEndOfFrame();
            }
            for (; tiempoUltimoGolpe < zoomTime*2; tiempoUltimoGolpe+=Time.deltaTime) //Se resetea cada vez que se activa el evento
            {
                yield return new WaitForEndOfFrame();
            }
                StartCoroutine(ZoomOut());

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

    public IEnumerator ZoomOut()
    {
        Debug.Log("ZoomOUT!!");
        float zoomActual = cv.m_Lens.OrthographicSize;
        float zoomPorSegundo = (zoomInicial - zoomActual) / (zoomTime) * Time.deltaTime;
        for (; zoomActual < zoomInicial; zoomActual += zoomPorSegundo)
        {
            cv.m_Lens.OrthographicSize = zoomActual;
            yield return new WaitForEndOfFrame();
        }
        zooming = false;
    }
}
