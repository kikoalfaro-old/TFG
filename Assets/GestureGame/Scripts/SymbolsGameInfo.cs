using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class LevelsDictionary : SerializableDictionary<Difficulty, Level> {}
// Diccionario de correspondencia Difficulty --> Level

[Serializable]
public enum Difficulty { Easy, Normal, Hard, Extreme }

[Serializable]
public struct SymbolSpawningPoint
{
    public RectTransform transform;
    private Vector2 direction;

    public Vector2 Direction
    {
        get
        {
            // La dirección siempre se mueve hacia el centro
            return new Vector2(-transform.localPosition.x, -transform.localPosition.y).normalized;
        }
    }

}


[Serializable]
public class Level
{
    [Header("Symbol speed parameters")]
    public float symbolMinSpeed;
    public float symbolMaxSpeed;
    [Space]
    public float timeBetweenSymbols;
    public int amountOfSymbolsToSpawn; // Cantidad total de símbolos que se spawnearán
    [Tooltip("Porcentaje de acierto mínimo para superar el nivel")]
    public int minTotalAccuracy; // Porcentaje de acierto mínimo para superar el nivel
    [Space]
    [Header("Spawning points")]
    public SymbolSpawningPoint[] spawningPoints;
    [Space]
    [Header("Ranges")]
    public RectTransform[] ranges; // Conjunto de rangos para este nivel
    [Space]
    [Header("Symbol movement")]
    [Tooltip("Arrastrar prefab con script SymbolMovement aquí")]
    public SymbolMovement symbolMov; // Información del script de movimiento de los símbolos
}
 
// Sólo vamos a usar esta clase como almacén de datos
public class SymbolsGameInfo : ScriptableObject
{
    public LevelsDictionary levelsInfo = new LevelsDictionary();

    private static SymbolsGameInfo instance;              // The singleton instance.

    private const string loadPath = "SymbolsGameInfo";    // The path within the Resources folder that 
    // Ver cómo está hecho para integrarlo bien

    public static SymbolsGameInfo Instance                // The public accessor for the singleton instance.
    {
        get
        {
            // If the instance is currently null, try to find a SymbolsGameInfo instance already in memory.
            if (!instance)
                instance = FindObjectOfType<SymbolsGameInfo>();
            // If the instance is still null, try to load it from the Resources folder.
            if (!instance)
                instance = Resources.Load<SymbolsGameInfo>(loadPath);
            // If the instance is still null, report that it has not been created yet.
            if (!instance)
                Debug.LogError("SymbolsGame no ha sido creado y no existe información.  Ves a  Assets > Create > Symbols Game Info");
            return instance;
        }
        set { instance = value; }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Symbols Game Info")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<SymbolsGameInfo>();
    }
#endif

    public static Level GetLevel(Difficulty difficulty)
    {
        return Instance.levelsInfo[difficulty]; //Devuelve la información del nivel de la dificultad dada
    }
}

