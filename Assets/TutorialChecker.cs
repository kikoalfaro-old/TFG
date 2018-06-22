using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialChecker : MonoBehaviour {

    public void LoadFirstScene()
    {
        if (PlayerPrefs.GetInt("FirstTime", 0) == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            SceneManager.LoadScene("Tutorial");
        }
        else SceneManager.LoadScene("Persistent");
    }
}
