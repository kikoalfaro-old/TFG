using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public enum AreaStatus { Unknown, Available, Visited, Completed };

[Serializable]
public class GameData // Aquí se guardan los ESTADOS de las áreas y la puntuación (dado el caso)
{
    // score
    public Dictionary<string, AreaStatus> areasStatus;

    public GameData(StringStringDictionary allAreas)
    {
        foreach (KeyValuePair<string, string> area in allAreas)
        {
            areasStatus.Add(area.Key, AreaStatus.Unknown); // Creamos el nuevo diccionario de estados
        }
    }
}

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
        string destination = Application.persistentDataPath + "/playerData.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData loadedGameData = (GameData)bf.Deserialize(file);
        file.Close();

        Debug.Log("Game loaded");
        return loadedGameData;
    }

}
