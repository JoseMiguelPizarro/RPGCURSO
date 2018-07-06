using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelRenderPrueba : MonoBehaviour {

    private CanvasRenderer miRender;

    private void Awake()
    {
        miRender = GetComponent<CanvasRenderer>();
        
    }

    IEnumerator FadeOut()
    {
        Debug.Log("Ejecutando shader");
        for (float i = 0; i <= 1; i+=0.03f)
        {
            miRender.GetMaterial().SetFloat("_Destroyer_Value_1", i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeIn()
    {
        Debug.Log("Ejecutando shad");
        for (float i = 1; i >= 0; i -= 0.03f)
        {
            miRender.GetMaterial().SetFloat("_Destroyer_Value_1", i);
            yield return new WaitForEndOfFrame();
        }
    }


    public void DesvanecerUI()
    {
        StartCoroutine(FadeOut());
    }

    public void AparecerUI()
    {
        StartCoroutine(FadeIn());
    }
}
