using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area  {

    public string nombre;
    public double latitud;
    public double longitud;
    public double radio;

    public Area(string n, double la, double lo, double rad)
    {
        this.nombre = n;
        this.latitud = la;
        this.longitud = lo;
        this.radio = rad;
    }

    public override string ToString()
    {
        return "Nombre: " + this.nombre + "\nLatitud: " + this.latitud + "; Longitud: " + this.longitud + "\nRadio: " + this.radio;
    }

}
