using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapManager : MonoBehaviour {


    // Diccionario serializado Status -> Color
    [SerializeField]
    public Color completedAreaColor;
    public Color visitedAreaColor;
    public Color availableAreaColor;
    public Color unknownAreaColor;

    [Space]
    public Image[] areaImages;
    GeoLocData geoLocData;

    private void OnEnable()
    {
        geoLocData = GeoLocData.Instance;
        SetAreaColors();
    }

    public void DisableMap()
    {
        gameObject.SetActive(false);
    }

    private void SetAreaColors() // Se pueden poner en amarillo si están visitadas pero no completadas
    {
        for (int i = 0; i < areaImages.Length; i++)
        {
            switch (geoLocData.allAreas[i + 1].Status)
            {
                case AreaStatus.Unknown:
                    areaImages[i].color = unknownAreaColor;
                    break;
                case AreaStatus.Available:
                    areaImages[i].color = availableAreaColor;
                    break;
                case AreaStatus.Visited:
                    areaImages[i].color = visitedAreaColor;
                    break;
                case AreaStatus.Completed:
                    areaImages[i].color = completedAreaColor;
                    break;
                default:
                    break;
            }
        }

        Debug.Log("Map colors updated");

    }
}
