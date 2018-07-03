using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityDialogoEvent : UnityEvent<Dialogo> { }
public class Dialogo:MonoBehaviour  {

    [TextArea(3,10)]
    public string[] frases;
    public string nombre;
    private GestorDialogo gestorDialogo;
    public UnityDialogoEvent OnDialogo;

    private void Awake()
    {
        gestorDialogo= FindObjectOfType<GestorDialogo>();
    }
    public void invocarDialogo()
    {
        gestorDialogo.StartDialogue(this);
    }
}
