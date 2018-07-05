using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TipoDeEquipamiento
{
    Arma,
    Ropaje,
    Botas,
    Accesorio,
    Casco,
    Guantes,
}

[CreateAssetMenu(menuName = "Objeto/Equipamiento")]
public class Equipamiento : Item {

    public TipoDeEquipamiento tipoEquipo;
    [Space]
    [Tooltip("Salud que entrega el objeto")]
    public int Salud;
    public int Magia;
    public int Inteligencia;
    public int Fuerza;
    public int Defensa;
    public int Velocidad;
    private string textoTooltip = "";



    public string StringAtributos()
    {
        Equipamiento equipamientoActual = equipamientoMismoTipo();  //Primero determina el equipo del mismo tipo actualmente equipado
        string descripcionString = "";
       

        if (Salud>0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Salud<Salud) //Atributo objeto actual es mayor que atributo en objeto equipado
                {
                    descripcionString += textoAtributo(Atributos.Salud, true);
                }
                else if (equipamientoActual.Salud>Salud)
                {
                    descripcionString += textoAtributo(Atributos.Salud, false);
                }
                else
                {
                    descripcionString += "\nSalud +" + Salud;
                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Salud, true);
            }
        }
        if (Magia>0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Magia < Magia)
                {
                    descripcionString += textoAtributo(Atributos.Magia, true);
                }
                else if (equipamientoActual.Magia>Magia)
                {
                    descripcionString += textoAtributo(Atributos.Magia, false);

                }
                else
                {
                    descripcionString += "\nMagia +" + Magia;
                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Magia, true);
            }
        }
        if (Inteligencia > 0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Inteligencia < Inteligencia)
                {
                    descripcionString += textoAtributo(Atributos.Inteligencia, true);
                }
                else if(equipamientoActual.Inteligencia>Inteligencia)
                {
                    descripcionString += textoAtributo(Atributos.Inteligencia, false);
                }
                else
                {
                    descripcionString += "\nInteligencia +" + Inteligencia;
                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Inteligencia, true);
            }

        }
        if (Fuerza>0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Fuerza < Fuerza)
                {
                    descripcionString += textoAtributo(Atributos.Fuerza, true);
                }
                else if(equipamientoActual.Fuerza>Fuerza)
                {
                    descripcionString += textoAtributo(Atributos.Fuerza, false);
                }
                else
                {
                    descripcionString += "\nFuerza +" + Fuerza;

                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Fuerza, true);
            }

        }

        if (Defensa>0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Defensa < Defensa)
                {
                    descripcionString += textoAtributo(Atributos.Defensa, true);
                }
                else if(equipamientoActual.Defensa>Defensa)
                {
                    descripcionString += textoAtributo(Atributos.Defensa, false);
                }
                else
                {
                    descripcionString += "\nDefensa +" + Defensa;

                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Defensa, true);
            }
           
        }
        if (Velocidad > 0)
        {
            if (equipamientoActual)
            {
                if (equipamientoActual.Velocidad < Velocidad)
                {
                    descripcionString += textoAtributo(Atributos.Velocidad, true);

                }
                else if (equipamientoActual.Velocidad>Velocidad)
                {
                    descripcionString += textoAtributo(Atributos.Velocidad, false);

                }
                else
                {
                    descripcionString += "\nVelocidad +" + Velocidad;
                }
            }
            else
            {
                descripcionString += textoAtributo(Atributos.Velocidad, true);
            }
        }

        descripcionString +=  "\n\n <i> <color=grey>\""+descripcion+ "\"</color> </i>";
        return descripcionString;
    }

    private Equipamiento equipamientoMismoTipo()
    {
        foreach (Equipamiento equipo in PanelEquipamiento.Equipamiento.equipamientos)
        {
            if (equipo.tipoEquipo==tipoEquipo)
            {
                return equipo;
            }
        }
        return null;
    }

    private string textoAtributo(Atributos atributos,bool mayor)
    {
        string color;
        if (mayor)
        {
            color = "<color=green>";
        }
        else
        {
            color = "<color=red>";
        }
        switch (atributos)
        {
            case Atributos.Salud:
                textoTooltip = string.Format( "\n{0} <b>Salud +"+Salud+ "</b></color>",color);
                break;
            case Atributos.Magia:
                 textoTooltip = string.Format("\n{0} <b>Magia +" + Magia + "</b></color>",color);
                break;
            case Atributos.Velocidad:
                 textoTooltip = string.Format("\n{0}<b> Velocidad +" + Velocidad + "</b></color>",color);
                break;
            case Atributos.Inteligencia:
                 textoTooltip = string.Format("\n{0} <b>Inteligencia +" + Inteligencia + "</b></color>",color);
                break;
            case Atributos.Fuerza:
                 textoTooltip = string.Format("\n{0}<b> Fuerza +" + Fuerza + "</b></color>",color);
                break;
            case Atributos.Defensa:
                textoTooltip = string.Format("\n{0}<b> Defensa +" + Defensa + "</b></color>",color);
                break;
            default:
                break;
        }
        return textoTooltip;
    }
}
