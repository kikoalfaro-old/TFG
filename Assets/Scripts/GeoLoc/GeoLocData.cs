using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Area
{
    public string name;
    public int id;
    public GeoLocCoordinates centre; //Mejor por una string spliteada (esto ocultar y la string serializar)
    bool visited = false;
    public bool completed = false;

    #region Properties
    public bool Completed
    {
        get
        {
            return completed;
        }

        set
        {
            completed = value;
        }
    }
    public bool Visited
    {
        get
        {
            return visited;
        }

        set
        {
            visited = value;
        }
    }
    #endregion

    public override string ToString()
    {
        return name;
    }    
}

public class GeoLocData : ScriptableObject
{
    public float areaRadius = 200f;

    public List<Area> allAreas;

    private static GeoLocData instance;              // The singleton instance.

    private const string loadPath = "GeoLocData";    // The path within the Resources folder that 
                                                          // Ver cómo está hecho para integrarlo bien

    public static GeoLocData Instance                // The public accessor for the singleton instance.
    {
        get
        {
            // If the instance is currently null, try to find a SymbolsGameInfo instance already in memory.
            if (!instance)
                instance = FindObjectOfType<GeoLocData>();
            // If the instance is still null, try to load it from the Resources folder.
            if (!instance)
                instance = Resources.Load<GeoLocData>(loadPath);
            // If the instance is still null, report that it has not been created yet.
            if (!instance)
                Debug.LogError("GeoLocData no ha sido creado y no existe información.  Ves a  Assets > Create > GeoLocData");
            return instance;
        }
        set { instance = value; }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/GeoLocData")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GeoLocData>();
    }
#endif

}

