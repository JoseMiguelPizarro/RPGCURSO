using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour {

    private TrailRenderer trail;
    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
    }
    public IEnumerator Dash(Vector2 direccionAtaque)
    {
        trail.enabled = true;
        Debug.Log("Dashing");
        for (int i = 0; i < 20; i++)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccionAtaque, 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        trail.enabled = false;
    }
}
