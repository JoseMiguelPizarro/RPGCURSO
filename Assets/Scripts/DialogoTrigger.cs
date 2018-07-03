using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogoTrigger : MonoBehaviour {

    public Dialogo diálogo;

	public void TriggerDialog()
    {
        FindObjectOfType<GestorDialogo>().StartDialogue(diálogo);
    }	
	
}
