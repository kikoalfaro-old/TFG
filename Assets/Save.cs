using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    // score
    List<Area> areasData;

    public GameData()
    {
        areasData = GeoLocData.Instance.allAreas; // Hace una copia del array de áreas para guardarlo
    }

    public void LoadData()
    {
        GeoLocData.Instance.allAreas = areasData;
    }
}

public class Save : MonoBehaviour
{
    public static Save instance = null;

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
        SaveGame();
        LoadGame();
    }

    public void SaveGame()
    {
        string destination = Application.persistentDataPath + "/playerData.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        GameData gameData = new GameData();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, gameData);
        file.Close();

        Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        string destination = Application.persistentDataPath + "/playerData.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData gameData = (GameData)bf.Deserialize(file);
        file.Close();

        gameData.LoadData(); // Carga de nuevo toda la información de las áras        

        Debug.Log("Game loaded");

    }

}
