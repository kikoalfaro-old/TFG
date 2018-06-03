using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public DebugButtonGenerator generator;

    private static GameManager instance = null;

    GeoLocManager geoLocManager; // Actualiza coordenadas, y llama a la carga escenas...
    public GeoLocData geoLocData; // Datos de las áreas
    SaveManager saveManager;

    GameData gameData; // Aquí es donde se almacenará toda la información de los estados de las áreas

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
        saveManager = GetComponent<SaveManager>();
        Debug.Log(Application.persistentDataPath);


        gameData = LoadGame();
        if (gameData == null) // NO HAY NINGÚN ARCHIVO GUARDADO PREVIAMENTE, POR LO QUE CREAMOS UNO
        {
            SaveGame(new GameData(geoLocData.allAreas)); // Creo el diccionario a partir del Scriptable y lo serializo
            LoadGame();
        }

        generator.GenerateButons();
    }

    public AreaStatus GetCurrentAreaStatus()
    {
        return gameData.areasStatus[geoLocManager.GetCurrentArea()];
    }

    public void UpdateCurrentAreaStatus(AreaStatus newStatus)
    {
        gameData.areasStatus[geoLocManager.GetCurrentArea()] = newStatus; // SUPONGAMOS QUE ASÍ SE ASIGNA UN VALOR A UNA CLAVE
        SaveGame(gameData); // Guardamos el juego después de actualizar el estado
        Debug.Log("Area status updated and game saved");
    }

    public void ResetAllStatus()
    {
        foreach (KeyValuePair<string, AreaStatus> area in gameData.areasStatus)
        {
            gameData.areasStatus[area.Key] = AreaStatus.Available; // OJO, HABRÁ QUE PONERLO A UNKNOWN EN LA VERSIÓN FINAL
        }

        SaveGame(gameData);
    }

    public void SaveGame(GameData gameData)
    {
        saveManager.SaveGame(gameData);
    }

    public GameData LoadGame()
    {
        return saveManager.LoadGame();
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    public GeoLocData GetGeoLocData()
    {
        return geoLocData;
    }
}
