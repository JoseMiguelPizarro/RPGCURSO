using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="objeto",menuName ="Item")]
public class Item : ScriptableObject {
    public new string name;
    public string descripcion;
    public Sprite artwokr;
    public int value;
    public bool Consumible;

    public void Usar()
    {
        Debug.Log("Cura 20 HP");
    }
}
