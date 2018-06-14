using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="objeto",menuName ="Item")]
public class Item : ScriptableObject {
    public new string name;
    public string descripcion;
    public Sprite artwokr;
    public int value;
    public bool apilable;

    public void UsarObjeto()
    {
        Debug.Log("Usando Objeto");
    }
}
