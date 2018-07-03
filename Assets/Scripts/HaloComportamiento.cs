using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloComportamiento : MonoBehaviour {

    public string sortingLayer;
    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = sortingLayer;
    }
}
