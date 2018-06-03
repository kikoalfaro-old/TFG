using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GeoLocData : ScriptableObject
{
    public float areaRadius = 200f;

    public StringStringDictionary allAreas = new StringStringDictionary(); // Un área está definida por un nombre y unas coordenadas

    private const string loadPath = "GeoLocData";    // The path within the Resources folder that 

    public static string LoadPath
    {
        get
        {
            return loadPath;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/GeoLocData")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<GeoLocData>();
    }
#endif

}

