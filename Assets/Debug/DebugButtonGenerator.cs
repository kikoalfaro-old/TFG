using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonGenerator : MonoBehaviour {

    public GameObject buttonPrefab;
    List<Area> areas;

    // Use this for initialization
    void Start () {
        StartCoroutine(SetButtons());
	}

    IEnumerator SetButtons()
    {
        yield return new WaitForEndOfFrame();
        areas = GeoLocData.Instance.allAreas;
        int numButtons = areas.Count;
        for (int i = 1; i < numButtons; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, transform) as GameObject;   
            newButton.GetComponent<Button>().interactable = true;                               
            newButton.GetComponentInChildren<Text>().text = areas[i].name;
            newButton.GetComponent<DebugButton>().coords = areas[i].centre.ToString();
        }

        yield return null;
    }

} 
