using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Jefe: MonoBehaviour  {
    private GestorDeSalud gestorDeSalud;
    public BarraSalud2 barraDeSalud;
    public UnityEvent OnComenzarBossBattle;
    private EnemigoAI enemigoAI;

    private void Start()
    {
        gestorDeSalud = GetComponent<GestorDeSalud>();
        enemigoAI = GetComponent<EnemigoAI>();

    }

    public void LlenarVidaHP()
    {
      StartCoroutine(CrecerBarraDeVida(barraDeSalud));
        Debug.Log("LlenarVidaHP");
    } 
	
  public  IEnumerator CrecerBarraDeVida(BarraSalud2 barraDeSalud)
    {
        Debug.Log("Llenando vida");
        for (float i = 0; i < 1; i+=0.01f)
        {
            barraDeSalud.transform.localScale = new Vector3(i, barraDeSalud.transform.localScale.y, 1);
            yield return new WaitForEndOfFrame();
        }
        barraDeSalud.transform.localScale = new Vector3(1, barraDeSalud.transform.localScale.y, 1);
    }
}
