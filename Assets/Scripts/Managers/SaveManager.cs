using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;

[Serializable]
public enum AreaStatus { Locked, Available, Visited, Completed };

[Serializable]
public class GameData // Aquí se guardan los ESTADOS de las áreas y la puntuación (dado el caso)
{
    public int completedPercentage;
    public Dictionary<string, AreaStatus> areasStatus;
    List<string> lockedAreas;

    public GameData(List<Area> allAreas)
    {
        areasStatus = new Dictionary<string, AreaStatus>();
        lockedAreas = new List<string>();
        Debug.Log("Creating new gameData object...  " + allAreas);

        int unknownAmount = Mathf.CeilToInt(allAreas.Count * (GameManager.Instance.unknownAreasPercentage / 100f)); // Número de áreas unknown (a la alza)
        foreach (Area area in allAreas)
        {
            if (area.name == GameManager.defaultAreaName) continue;

            areasStatus.Add(area.name, AreaStatus.Available); // Creamos el nuevo diccionario de estados
        }

        int unknownAreas = unknownAmount;
        while(unknownAreas > 0)
        {
            string areaToChange = areasStatus.ElementAt(UnityEngine.Random.Range(0, areasStatus.Count-1)).Key;
            if (areasStatus[areaToChange] == AreaStatus.Available)
            {
                areasStatus[areaToChange] = AreaStatus.Locked;
                lockedAreas.Add(areaToChange); //Añado el nombre del área para acceder a ella rápido
                Debug.Log("--> Cambiada el área " + areaToChange + " a Unknown");
                unknownAreas--;
            }
        }


        completedPercentage = 0; //Reinicio el porcentaje completado
    }

    public void AddArea(Area area)
    {
        if (area.name != GameManager.defaultAreaName)
        {
            areasStatus.Add(area.name, AreaStatus.Available); // Creamos el nuevo diccionario de estados
            UpdateCompletedPercentage();
        }
    }

    public void RemoveArea(Area area)
    {
        areasStatus.Remove(area.name);
        UpdateCompletedPercentage();
    }

    public void RemoveArea(string areaName)
    {
        areasStatus.Remove(areaName);
        Debug.Log("Removed: " + areaName);
        UpdateCompletedPercentage();
    }

    public void UpdateCompletedPercentage()
    {
        Debug.Log("Updating % ...");
        float visited = 0;
        float completed = 0;

        float completedCost = 100 / areasStatus.Count;
        float visitedCost = completedCost * 0.5f;

        foreach (KeyValuePair<string, AreaStatus> area in areasStatus)
        {
            if (area.Value == AreaStatus.Visited) visited++;
            if (area.Value == AreaStatus.Completed) completed++;
        }

        completedPercentage = Mathf.RoundToInt(visited * visitedCost + completed * completedCost);

        // Actualizo la leaderboard con el porcentaje completado
        GPGSManager.Instance.AddScoreLeaderBoard(completedPercentage);
    }

    /// <summary>
    /// En caso de que queden áreas bloquedas, se desbloquea un área aleatoriamente y se quita de la lista
    /// </summary>
    public void UnlockRandomArea()
    {
        if(lockedAreas.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, lockedAreas.Count - 1);
            string areaToUnlock = lockedAreas[randomIndex];
            areasStatus[areaToUnlock] = AreaStatus.Available;
            lockedAreas.Remove(areaToUnlock);
        }
    }
}


/// <summary>
/// Se encarga de manejar los datos tanto localmente como en la nube
/// </summary>
public class SaveManager : MonoBehaviour
{

    private static SaveManager instance = null;

    public static SaveManager Instance
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


    public void SaveGame(GameData gameDataToSave) // Le pasamos el objeto de gameData que deseamos serializar
    {
        Debug.Log("gameDataToSave " + gameDataToSave);
        string destination = Application.persistentDataPath + "/playerData.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, gameDataToSave);
        file.Close();

        Debug.Log("Game saved");
    }

    public GameData LoadGame()
    {
        Debug.Log("Loading game");
        string destination = Application.persistentDataPath + "/playerData.dat";
        FileStream file;

        Debug.Log("File exists: " + File.Exists(destination));

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.Log("File not exists, so it returns null");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData loadedGameData = (GameData)bf.Deserialize(file);
        file.Close();

        Debug.Log("Game loaded");
        return loadedGameData;
    }
}
