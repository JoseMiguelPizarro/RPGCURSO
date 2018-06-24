using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="objeto",menuName ="Objeto/Item")]
public class Item : ScriptableObject {
    public new string name;
    public string descripcion;
    public Sprite artwork;
    public int value;
    public bool apilable;

   virtual public bool UsarObjeto()
    {
        Debug.Log("Usando Objeto");
        return true;
    }
}
