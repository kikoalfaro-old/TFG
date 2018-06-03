using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MapManager : MonoBehaviour {
    
    [SerializeField]
    AreaStatusColorDictionary statusColors = new AreaStatusColorDictionary();

    [Space]
    [SerializeField]
    StringImageDictionary areaImages = new StringImageDictionary();

    GameData gameData;

    // Esto funciona solo porque en el primer frame (cuando se cogen todas las referencias), el mapa estará desactivado
    private void OnEnable()
    {
        gameData = GameManager.Instance.GetGameData();
        SetAreaColors();
    }

    public void DisableMap()
    {
        gameObject.SetActive(false);
    }

    private void SetAreaColors() // Se pueden poner en amarillo si están visitadas pero no completadas
    {
        foreach (KeyValuePair<string, AreaStatus> area in gameData.areasStatus)
        {
            areaImages[area.Key].color = statusColors[area.Value]; // Devuelve el color que corresponde a ese estado
        }
    }
}
