using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour {
    [SerializeField] float dashSpeed = 10;
    public float dashCD = 1;
    private TrailRenderer trail;
    [HideInInspector]
    public bool dashing = false;
    [HideInInspector]
    public bool dashReady = true;
    public float dashDuration = 0.2f;
    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public IEnumerator Dash(Vector2 direccionAtaque)
    {
        if (dashReady)
        {
            dashing = true;
            dashReady = false;
            trail.enabled = true;
            Debug.Log("Dashing");
            for (int i = 0; i < dashDuration*60; i++)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)direccionAtaque, dashSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            dashing = false;
            yield return new WaitForSeconds(trail.time);
            trail.enabled = false;
            yield return new WaitForSeconds(dashCD);
            dashReady = true;
        }
    }
}
