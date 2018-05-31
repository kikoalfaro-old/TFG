using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void DisableMap()
    {
        gameObject.SetActive(false);
    }
}
