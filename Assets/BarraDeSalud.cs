using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraDeSalud : MonoBehaviour {

	public void Actualizar(float porcentaje)
    {
        transform.localScale = new Vector3(porcentaje, transform.localScale.y);
    }
}
