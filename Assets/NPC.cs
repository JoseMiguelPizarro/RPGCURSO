using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogo),typeof(CapsuleCollider2D),typeof(Rigidbody2D))]

public class NPC : Interactivo {

    private Dialogo dialogo;
	// Use this for initialization
	void Awake () {
        dialogo= GetComponent<Dialogo>();
        Inicializar();
	}

    protected override void Interaccion()
    {
        dialogo.invocarDialogo();
    }
}
