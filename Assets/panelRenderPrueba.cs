using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class panelRenderPrueba : MonoBehaviour {

    private CanvasRenderer myRender;

    private void Awake()
    {
        myRender = GetComponent<CanvasRenderer>();
        
    }

    IEnumerator FadeOut()
    {
        Debug.Log("Ejecutando shader");
        for (float i = 0; i <= 1; i+=0.03f)
        {
            myRender.GetMaterial().SetFloat("_Burn_Value_1", i);
            yield return new WaitForEndOfFrame();
        }
        myRender.GetMaterial().SetFloat("_Burn_Value_1", 1);
        myRender.GetMaterial().SetFloat("_SpriteFade", 0f);
        Debug.Log("spritefade es" + myRender.GetMaterial().GetFloat("_SpriteFade"));

    }


    IEnumerator FadeIn()
    {
        Debug.Log("Ejecutando shad");
        myRender.GetMaterial().SetFloat("_SpriteFade", 1f);

        for (float i = 1; i >= 0; i -= 0.03f)
        {
            myRender.GetMaterial().SetFloat("_Burn_Value_1", i);
            yield return new WaitForEndOfFrame();
        }
        myRender.GetMaterial().SetFloat("_Burn_Value_1", 0);
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
