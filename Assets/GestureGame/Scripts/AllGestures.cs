using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Symbol
{
    // Información sobre el símbolo
    [SerializeField]
    public string symbolName;
    [SerializeField]
    public Sprite symbolSprite;
}

public class AllGestures : ScriptableObject
{
    public Symbol[] allSymbols;

    private static AllGestures instance;              // The singleton instance.


    private const string loadPath = "AllGestures";    // The path within the Resources folder that 
    // Ver cómo está hecho para integrarlo bien

    public static AllGestures Instance                // The public accessor for the singleton instance.
    {
        get
        {
            // If the instance is currently null, try to find a SymbolsGameInfo instance already in memory.
            if (!instance)
                instance = FindObjectOfType<AllGestures>();
            // If the instance is still null, try to load it from the Resources folder.
            if (!instance)
                instance = Resources.Load<AllGestures>(loadPath);
            // If the instance is still null, report that it has not been created yet.
            if (!instance)
                Debug.LogError("AllGestures no ha sido creado y no existe información.  Ves a  Assets > Create > Symbols Game Info");
            return instance;
        }
        set { instance = value; }
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Gestures Info")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<AllGestures>();
    }
#endif

    public static Symbol GetRandomSymbol()
    {
        int randomIndex = UnityEngine.Random.Range(0, Instance.allSymbols.Length);
        return Instance.allSymbols[randomIndex];
    }

}
