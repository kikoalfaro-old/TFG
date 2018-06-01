using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

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
            areaImages[i].color = (geoLocData.allAreas[i].Completed) ? completedAreaColor : availableAreaColor;
        }

        Debug.Log("Map colors updated");

    }
}
