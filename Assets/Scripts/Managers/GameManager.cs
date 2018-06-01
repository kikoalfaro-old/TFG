using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    GeoLocManager geoLocManager; // Actualiza coordenadas, y llama a la carga escenas...
    GeoLocData geoLocData; // Datos de las áreas

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);    
    }

    private void Start()
    {
        geoLocManager = GeoLocManager.Instance;
    }

    // Esto quizá habria que llamarlo con un delegate de evento desde cualquier lado. --> marca el área actual como completa y guarda los datos del juego
    public void OnAreaCompleted()
    {
        geoLocData.allAreas[geoLocManager.GetCurrentArea().id].Completed = true;
    }

    public void OnAreaVisited()
    {
        geoLocData.allAreas[geoLocManager.GetCurrentArea().id].Visited = true;
    }
}
