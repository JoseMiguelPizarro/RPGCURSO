using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class GestorDialogo : MonoBehaviour {

    public Text nameText;
    public Text dialogoTexto;
    public float delay = 0.1f;
    public UnityEvent OnEndDialogo;

    private string currentText = "";
    private string oración = "";
    private Queue<string> frases; //FIFO 
    private Coroutine textoanimado;
    private CanvasGroup canvas;



	void Start () {
        frases = new Queue<string>();
        dialogoTexto.text = "";
        canvas = GetComponent<CanvasGroup>();
	}

    //public void  StartDialogue(Dialogo dialogo)
    //{
    //    Debug.Log("Iniciando dialogo");
    //    canvas.interactable = true;
    //    canvas.alpha = 1;
    //    canvas.blocksRaycasts = true;

    //    nameText.text = dialogo.nombre;
    //    frases.Clear();
    //    foreach(string oraciones in dialogo.frases)
    //    {
    //        frases.Enqueue(oraciones);
    //    }

    //    DisplayNextSentence();
    //}

    //public void DisplayNextSentence()
    //{
    //   //StopCoroutine(textoanimado);
    //    if (frases.Count == 0 /*&& oración.Length==dialogoTexto.text.Length*/)
    //    {

    //        EndDialogue();
    //        return;
    //    }
    //    //if (oración.Length == dialogoTexto.text.Length)
    //    //{
    //    //  // textoanimado= StartCoroutine(ShowText(oración));

    //    //}
    //    else {
    //        //   StopCoroutine(textoanimado);
    //        oración = frases.Dequeue();
    //        dialogoTexto.text = oración; }
    //    //dialogoTexto.text = oración;
    //}

    //void EndDialogue()
    //{
    //    //StopCoroutine(textoanimado);
    //    frases.Clear();
    //    canvas.interactable = false;
    //    canvas.alpha = 0;
    //    canvas.blocksRaycasts = false;
    //}

    //IEnumerator ShowText(string texto)
    //{
    //    for (int i = 0; i <= texto.Length; i++)
    //    {
    //        currentText = texto.Substring(0, i);
    //        audioSource.clip = audiotexto[Random.Range(0,audiotexto.Length-1)];
    //        dialogoTexto.text = currentText;
    //        audioSource.Play();
    //        if (currentText.Length == texto.Length)
    //        {
    //            break;
    //        }
    //            yield return new WaitForSeconds(delay);
    //    }
    //}

    public void StartDialogue(Dialogo dialogo)
    {
        Debug.Log("Iniciando dialogo");
        canvas.interactable = true;
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
        nameText.text = dialogo.nombre;
        frases.Clear();
        foreach (string oraciones in dialogo.frases)
        {
            frases.Enqueue(oraciones);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        //StopCoroutine(textoanimado);
        if (frases.Count == 0 && oración.Length == dialogoTexto.text.Length)
        {

            EndDialogue();
            return;
        }
        if (oración.Length == dialogoTexto.text.Length)
        {
            oración = frases.Dequeue();
            textoanimado = StartCoroutine(ShowText(oración));

        }
        else
        {
            StopCoroutine(textoanimado);
            dialogoTexto.text = oración;
        }
        //dialogoTexto.text = oración;
    }

    void EndDialogue()
    {
        StopCoroutine(textoanimado);
        frases.Clear();
        canvas.interactable = false;
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        OnEndDialogo?.Invoke();
    }

    IEnumerator ShowText(string texto)
    {
        for (int i = 0; i <= texto.Length; i++)
        {
            currentText = texto.Substring(0, i);
            dialogoTexto.text = currentText;
            if (currentText.Length == texto.Length)
            {
                break;
            }
            yield return new WaitForSeconds(delay);
        }

    }


}
