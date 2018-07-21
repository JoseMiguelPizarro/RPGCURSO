using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraSalud2 : MonoBehaviour {

	public void Actualizar(float razon)
    {
        transform.localScale = new Vector3(razon, transform.localScale.y, 1);
    }
}
