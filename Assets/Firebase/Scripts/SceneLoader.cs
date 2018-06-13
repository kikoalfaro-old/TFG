using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public void cargarEscenaDos()
    {
        SceneManager.LoadScene("EscenaDatos");

    }

    public void VolverAInicio()
    {
        SceneManager.LoadScene("Inicio");
    }

}
