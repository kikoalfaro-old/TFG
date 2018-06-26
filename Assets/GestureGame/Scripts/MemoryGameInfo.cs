using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[Serializable]
public class MemoryLevel
{
    [Header("Stage Parameters")]
    public int initialLevel; //Indica los elementos a pulsar seguidos (habrá que hacer el temporizador en función a esto también)
    [Tooltip("Número total de rondas")]
    public int stages; // Número total de rondas
}

// Sólo vamos a usar esta clase como almacén de datos
public class MemoryGameInfo : ScriptableObject
{
    public DifficultyMemoryLevelDictionary levelsInfo = new DifficultyMemoryLevelDictionary();

    private static MemoryGameInfo instance;              // The singleton instance.

    private const string loadPath = "MemoryGameInfo";    // The path within the Resources folder that 
                                                          // Ver cómo está hecho para integrarlo bien

    public static MemoryGameInfo Instance                // The public accessor for the singleton instance.
    {
        get
        {
            // If the instance is currently null, try to find a SymbolsGameInfo instance already in memory.
            if (!instance)
                instance = FindObjectOfType<MemoryGameInfo>();
            // If the instance is still null, try to load it from the Resources folder.
            if (!instance)
                instance = Resources.Load<MemoryGameInfo>(loadPath);
            // If the instance is still null, report that it has not been created yet.
            if (!instance)
                Debug.LogError("MemoryGameInfo no ha sido creado y no existe información.  Ves a  Assets > Create > Symbols Game Info");
            return instance;
        }
        set { instance = value; }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Memory Game Info")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<MemoryGameInfo>();
    }
#endif

    public static MemoryLevel GetLevel(Difficulty difficulty)
    {
        return Instance.levelsInfo[difficulty]; //Devuelve la información del nivel de la dificultad dada
    }
}

