using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookshot : MonoBehaviour {

    public GameObject nodoPrefab;
    public GameObject anclaPrefab;
    public LayerMask layers;
    public int seccionesCadena;
    public float largoSeccion;
    public int fuerzaArrastrar;

    private Vector2 posicionMouse;
    private Vector2 direccionMirada;
    private Vector2 posicionAjustada;
    private Vector2 vectorMirada;
    private Vector2 posicionJugador;
    private bool disparado;
    private bool disparando;
    private RaycastHit2D hit;
    private bool anclado;
    private Vector2 posicionCuerda;
    private int seccionActual;
    private List<GameObject> nodos = new List<GameObject>();
    private LineRenderer cadena;
    private int seccionesMaximo;
    private GameObject ancla;

    private float tiempoInvulnerabilidad = 0.2f;
    private float contador = 0;

    private void Start()
    {
        cadena = GetComponent<LineRenderer>();
        cadena.positionCount = seccionesCadena;
        seccionActual = seccionesCadena - 1;
        seccionesMaximo = seccionesCadena;
        ancla = Instantiate(anclaPrefab, transform);
        
        Inicializar();
    }

    private void Inicializar()
    {
        for (int i = 0; i < seccionesCadena; i++)
        {
            GameObject tempNodo = Instantiate(nodoPrefab, transform);
            if (i == 0)
            {
               tempNodo.GetComponent<DistanceJoint2D>().connectedBody = ancla.GetComponent<Rigidbody2D>();
               //tempNodo.transform.position += Vector3.right * 2;
            }
            else
            {
                tempNodo.GetComponent<DistanceJoint2D>().connectedBody = nodos[i - 1].GetComponent<Rigidbody2D>();
            }
            tempNodo.GetComponent<DistanceJoint2D>().distance = largoSeccion;
            tempNodo.GetComponent<DistanceJoint2D>().maxDistanceOnly = false;
            nodos.Add(tempNodo);
        }
    }

    private void Update()
    {
        
        posicionJugador = transform.position;
        posicionMouse = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        direccionMirada = posicionMouse - (Vector2)transform.position;
        float anguloMirada = Mathf.Atan2(direccionMirada.y, direccionMirada.x);
        float anguloMiradaDeg = anguloMirada * Mathf.Rad2Deg;
        vectorMirada = new Vector2(Mathf.Cos(anguloMirada), Mathf.Sin(anguloMirada));

        float longitud = (posicionJugador - posicionMouse).magnitude;
        AjustarCadena(longitud);
        Vector2 desfase = vectorMirada * largoSeccion * (nodos.Count);
        Vector2 posicionCuerda = posicionJugador + desfase;
        float posicionX, posicionY;
        if (posicionCuerda.x < posicionJugador.x)
            posicionX = Mathf.Clamp(posicionMouse.x, posicionCuerda.x, posicionJugador.x);
        else
            posicionX = Mathf.Clamp(posicionMouse.x, posicionJugador.x, posicionCuerda.x);

        if (posicionCuerda.y < posicionJugador.y)
            posicionY = Mathf.Clamp(posicionMouse.y, posicionCuerda.y, posicionJugador.y);
        else
            posicionY = Mathf.Clamp(posicionMouse.y, posicionJugador.y, posicionCuerda.y);

         posicionAjustada = new Vector2(posicionX, posicionY);

        ReconocerInput(posicionMouse);
        if (contador >= tiempoInvulnerabilidad)
        {
            //ActivarCollidersNodos();
            disparado = false;
            anclado = false;
            contador = 0;
            Physics2D.IgnoreCollision(hit.collider, GetComponentInParent<Collider2D>(), false);
        }

        if (disparado)
        {
            contador += Time.deltaTime;
        }
    }

    private void  FixedUpdate()
    {
        
        ActualizarCuerda(nodos);
       ancla.transform.position =transform.position+ (Vector3)vectorMirada * 0.5f; //0.5 es porque la circunferencia es de radio 0.5

            nodos[seccionesCadena-1].GetComponent<Rigidbody2D>().MovePosition (posicionAjustada);

        if (!disparado && anclado)
        {
           // hit.collider.gameObject.GetComponent<Rigidbody2D>().MovePosition(nodos[seccionesCadena - 1].transform.position);
            hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(direccionMirada*fuerzaArrastrar);
            if ((hit.transform.position-transform.position).magnitude>seccionesMaximo*largoSeccion*3)
            {
                ResetearCadena();
            }
            nodos[seccionesCadena - 1].GetComponent<Rigidbody2D>().MovePosition(hit.transform.position);
        }
        else
        {
            DetectarColision(posicionMouse);
        }
       
        if (disparado && anclado && disparando)
        {
            hit.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(direccionMirada.normalized * 30000);
            disparando = false;
            Debug.Log("Disparado!");
        }
    }

    private void ResetearCadena()
    {
        anclado = false;
        disparado = false;
        disparando = false;
        Physics2D.IgnoreCollision(hit.collider, GetComponentInParent<Collider2D>(), false);
    }

    private void AjustarCadena(float longitud)
    {

        int cantidadDeNodos =Mathf.CeilToInt((Mathf.Clamp(longitud, 0, largoSeccion * seccionesMaximo)/(nodoPrefab.GetComponent<DistanceJoint2D>().distance)));
        if (cantidadDeNodos>seccionesCadena)
        {
            for (int i = seccionesCadena; i < cantidadDeNodos; i++)
            {
                GameObject tempNodo = Instantiate(nodoPrefab, transform);
                tempNodo.GetComponent<DistanceJoint2D>().connectedBody = nodos[i-1].GetComponent<Rigidbody2D>(); //debe ser i puesto que este hace referencia al nodo actual, el nuevo creardo es i+1
                tempNodo.GetComponent<DistanceJoint2D>().distance = largoSeccion;
                tempNodo.GetComponent<DistanceJoint2D>().maxDistanceOnly = false;
                nodos.Add(tempNodo);
                Debug.Log("nodo añadido");
            }
        }
        if (cantidadDeNodos<seccionesCadena)
        {
            for (int i = seccionesCadena; i > cantidadDeNodos; i--)
            {
                Destroy(nodos[i - 1]);
                nodos.RemoveAt(i-1);
                Debug.Log("Nodo retirado");
            }
        }
        seccionesCadena = cantidadDeNodos;
    }

    private void ActualizarCuerda(List<GameObject> nodos)
    {
        cadena.positionCount = nodos.Count;
        for (int i = 0; i < cadena.positionCount; i++)
        {
            if (i==0)
            {
                cadena.SetPosition(i, transform.position);
            }
            else
            {
                cadena.SetPosition(i, nodos[i].transform.position);
            }
        }
    }

    private void ReconocerInput(Vector2 posicionMouse)
    {
        if (Input.GetMouseButtonDown(1) && anclado)
        {
            disparado = true;
            disparando = true;
            Debug.Log("Desancló");
        }
    }


    private void DetectarColision(Vector2 posicionMouse)
    {
        if (anclado)
            return;
        hit = Physics2D.CircleCast(transform.position,0.2f, direccionMirada,seccionesCadena*largoSeccion, layers);
        Debug.DrawRay(transform.position, direccionMirada.normalized * seccionesCadena * largoSeccion,Color.green);
        //AjustarCadena(100);
        if (hit) //Hacer nada si colisiona con muralla
        {
            Debug.Log("Hiteo");
            anclado = true;
            Physics2D.IgnoreCollision(hit.collider, GetComponentInParent<Collider2D>());
            //nada
        }
    }
}
