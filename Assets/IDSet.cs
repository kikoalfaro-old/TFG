using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

public class IDSet : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Flowchart>().SetStringVariable("ID", SceneManager.GetActiveScene().name);
	}
}
