using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static string defaultAreaName = "Default"; // Nombre del área por defecto
    [Range(0, 75)]
    public float unknownAreasPercentage = 25f; // Porcentaje de áreas que están en estado desconocido al inicio de una partida
    public AreaStatus unlockStatus = AreaStatus.Available;
    public string URL = "https://firestore.googleapis.com/v1beta1/projects/tfg-kiko-web/databases/(default)/documents/Areas";
    
    private static GameManager instance = null;

    GeoLocManager geoLocManager; // Actualiza coordenadas, y llama a la carga escenas...
                                 //public GeoLocData geoLocData; // Datos de las áreas

    public DebugButtonGenerator generator;
    List<Area> allAreas;
    public Difficulty currentDifficulty = Difficulty.Easy;

    SaveManager saveManager;
    GameData gameData; // Aquí es donde se almacenará toda la información de los estados de las áreas

    public event Action OnDataLoaded;

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

    IEnumerator Start()
    {
        allAreas = new List<Area>
        {
            new Area(defaultAreaName, 0, 0, 0, "") // Adding default area
        };

        using (WWW www = new WWW(URL))
        {
            yield return www; // It waits until the download is ended
            var content = JSON.Parse(www.text);
            for (int i = 0; i < content["documents"].Count; i++)
            {
                string name = content["documents"][i]["fields"]["nombre"]["stringValue"];
                double lat = content["documents"][i]["fields"]["latitud"]["doubleValue"];
                double lon = content["documents"][i]["fields"]["longitud"]["doubleValue"];
                double rad = content["documents"][i]["fields"]["radio"]["doubleValue"];
                string lab = content["documents"][i]["fields"]["etiqueta"]["stringValue"];
                Area a = new Area(name, lat, lon, rad, lab);
                allAreas.Add(a); // Adding each area that has been configured in the web app
            }

            GetExternalReferences();
            SyncronizeGameData();
            geoLocManager.StartGeoLoc();
        }        
    }

    private void SyncronizeGameData()
    {
        gameData = LoadGame();

        if (gameData == null) // NO HAY NINGÚN ARCHIVO GUARDADO PREVIAMENTE, POR LO QUE CREAMOS UNO (Va a estar actualizado siempre)
        {
            SaveGame(new GameData(allAreas)); // Se crean los estados de todas las áreas (Menos Defecto)
            gameData = LoadGame();
        }
        else // Si ya hay uno creado, debemos asegurarnos de que se actualice con el servidor
        {
            UpdateWithJSONData();
            SaveGame(gameData);
        }

        // Después de esto, el gameData ya que se queda actualizado con lo que hay en Internés

        //Debug.Log(Application.persistentDataPath);
    }

    // OBVIAMENTE esta no es la mejor opción. Un diccionario de <Area, AreaStatus> para las áreas bastaría para hacerlo más simple, pero :)
    private void UpdateWithJSONData()
    {
        // Primero añado las que están en la nube y no localmente
        foreach (Area area in allAreas) // Recorro las áreas cogidas
        {
            if (!gameData.areasStatus.ContainsKey(area.name)) gameData.AddArea(area); // Si no la tiene, la añado
        }

        // Guardo los nombres de las que están localmente y no están en la nube
        List<string> areasToRemove = new List<string>();
        foreach (KeyValuePair<string, AreaStatus> localArea in gameData.areasStatus)
        {
            bool isOnDatabase = false;
            foreach (Area area in allAreas) // Para cada local, miro si está en el json
            {
                if (area.name == localArea.Key)
                {
                    isOnDatabase = true;
                    break;
                }
            }

            if(!isOnDatabase) areasToRemove.Add(localArea.Key); // Si se han recorrido todas las de la nube y no está, debe eliminarse.
        }

        // Recorro cada una de las áreas a eliminar y las quito
        for (int i = 0; i < areasToRemove.Count; i++)
        {
            gameData.RemoveArea(areasToRemove[i]);
        }
    }

    private void GetExternalReferences()
    {
        geoLocManager = GeoLocManager.Instance;
        saveManager = GetComponent<SaveManager>();

        /*if (geoLocManager.debugMode)*/ generator.GenerateButons();
    }

    public AreaStatus GetCurrentAreaStatus()
    {
        return gameData.areasStatus[geoLocManager.GetCurrentArea().name];
    }

    public void UpdateCurrentAreaStatus(AreaStatus newStatus)
    {
        // La primera vez que se completa un área, se desbloquea un logro!
        if (newStatus == AreaStatus.Completed) GPGSManager.Instance.AchievementAccomplished(GPGSIds.achievement_madera_de_explorador);
        gameData.areasStatus[geoLocManager.GetCurrentArea().name] = newStatus; // SUPONGAMOS QUE ASÍ SE ASIGNA UN VALOR A UNA CLAVE
        gameData.UpdateCompletedPercentage(); // Actualizamos porcentaje completado
        if (newStatus == unlockStatus) gameData.UnlockRandomArea();
        SaveGame(gameData); // Guardamos el juego después de actualizar el estado
        Debug.Log("Area status updated and game saved");
    }
    
    public bool IsAreaLocked(Area area)
    {
        return gameData.areasStatus[area.name] == AreaStatus.Locked;
    }
    
    public void ResetAllStatus()
    {
        PlayerPrefs.SetInt("FirstTime", 0); // Se ve también el tutorial la próxima vez que se inicia
        SaveGame(new GameData(allAreas));
        gameData = LoadGame();
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

    public List<Area> GetAllAreas()
    {
        return allAreas;
    }

    public Difficulty GetCurrentDifficulty()
    {
        return currentDifficulty;
    }
}
