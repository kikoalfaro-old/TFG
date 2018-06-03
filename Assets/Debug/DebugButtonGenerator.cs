using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonGenerator : MonoBehaviour {

    public GameObject buttonPrefab;

    public void GenerateButons()
    {
        StringStringDictionary allAreas = GameManager.Instance.GetGeoLocData().allAreas;

        foreach (KeyValuePair<string, string> area in allAreas)
        {
            if (area.Key == GameManager.defaultAreaName) continue;
            GameObject newButton = Instantiate(buttonPrefab, transform);
            newButton.GetComponent<Button>().interactable = true;
            newButton.GetComponentInChildren<Text>().text = area.Key;
            newButton.GetComponent<DebugButton>().coords = allAreas[area.Key];
        }
    }
} 
