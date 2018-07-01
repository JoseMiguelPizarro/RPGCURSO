using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelEstado : MonoBehaviour {


	public Text nivel;
	public Text exp;
	public Text salud;
	public Text magia;
	public Text Fuerza;
	public Text Velocidad;
	public Text Inteligencia;
    public Text PuntosAtributo;

	public static PanelEstado panelEstado;

	private void Awake()
	{
		panelEstado = this;
	}
	// Use this for initialization
	void Start ()
	{
		ActualizarTextos();
	}

	public void ActualizarTextos()
	{
		nivel.text = AtributosJugador.atributosJugador.Nivel.ToString();
		exp.text = AtributosJugador.atributosJugador.Experiencia.ToString();
		salud.text = AtributosJugador.atributosJugador.Salud.ToString();
		magia.text = AtributosJugador.atributosJugador.Magia.ToString();
		Fuerza.text = AtributosJugador.atributosJugador.Fuerza.ToString();
		Velocidad.text = AtributosJugador.atributosJugador.Velocidad.ToString();
		Inteligencia.text = AtributosJugador.atributosJugador.Inteligencia.ToString();
        PuntosAtributo.text = AtributosJugador.atributosJugador.PuntosAtributos.ToString();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
