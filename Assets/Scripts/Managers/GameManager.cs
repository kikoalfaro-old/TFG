using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static GameManager instance = null;

    GeoLocManager geoLocManager; // Actualiza coordenadas, y llama a la carga escenas...
    GeoLocData geoLocData; // Datos de las áreas
    SaveManager saveManager;

    public static GameManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);    
    }

    private void Start()
    {
        geoLocManager = GeoLocManager.Instance;
        geoLocData = GeoLocData.Instance;
        saveManager = SaveManager.Instance;
    }

    public AreaStatus GetCurrentAreaStatus()
    {
        return geoLocManager.GetCurrentArea().Status;
    }

    public void UpdateCurrentAreaStatus(AreaStatus newStatus)
    {
        geoLocData.allAreas[geoLocManager.GetCurrentArea().id].Status = newStatus;
        //saveManager.SaveGame(); // Guardamos el juego después de actualizar el estado
        Debug.Log("Area status updated");
    }

    public void ResetAllStatus()
    {
        foreach (Area area in geoLocData.allAreas)
        {
            area.Status = AreaStatus.Available; // OJO, HABRÁ QUE PONERLO A UNKNOWN EN LA VERSIÓN FINAL
        }

        SaveGame();
    }

    public void SaveGame()
    {
        saveManager.SaveGame();
    }

    public void LoadGame()
    {
        saveManager.LoadGame();
    }
}
