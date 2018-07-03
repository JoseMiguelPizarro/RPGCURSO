using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject spawn;
    private float cd = 0;
    public float tiempoSpawn = 2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (cd>=tiempoSpawn)
        {
            Instantiate(spawn,transform.position,Quaternion.identity,transform);
            cd = 0;
        }
        cd += Time.deltaTime;
	}
}
