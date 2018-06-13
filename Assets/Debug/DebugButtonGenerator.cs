using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonGenerator : MonoBehaviour {

    public GameObject buttonPrefab;

    public void GenerateButons()
    {
        List<Area> allAreas = GameManager.Instance.GetAllAreas();

        foreach (Area area in allAreas)
        {
            if (area.name == GameManager.defaultAreaName) continue;
            GameObject newButton = Instantiate(buttonPrefab, transform);
            newButton.GetComponent<Button>().interactable = true;
            newButton.GetComponentInChildren<Text>().text = area.name;
            newButton.GetComponent<DebugButton>().coords = area.centre.ToString();
        }
    }
} 
