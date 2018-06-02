using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour {

    public string coords;

    // Llamar desde listener
    public void UpdateCurrentCoords()
    {
        GeoLocManager.Instance.currentCoords_Debug = coords;
    }
}
