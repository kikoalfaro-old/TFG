using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonGenerator : MonoBehaviour {

    public GameObject buttonPrefab;

    // Use this for initialization
    void Start () {
        StartCoroutine(SetButtons());
	}

    IEnumerator SetButtons()
    {
        yield return new WaitForEndOfFrame();
        StringStringDictionary allAreas = GameManager.Instance.GetGeoLocData().allAreas;

        foreach (KeyValuePair<string, string> area in allAreas)
        {
            GameObject newButton = Instantiate(buttonPrefab, transform) as GameObject;
            newButton.GetComponent<Button>().interactable = true;
            newButton.GetComponentInChildren<Text>().text = allAreas[area.Key];
            newButton.GetComponent<DebugButton>().coords = allAreas[area.Value];
        }

        yield return null;
    }
} 
