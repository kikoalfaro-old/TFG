using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultAreaManager : MonoBehaviour {

    public GameObject loadCanvas;
    public GameObject notZoneText;
    private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.
    private GeoLocManager geoLocManager;

    // Use this for initialization
    void Start () {

        sceneController = FindObjectOfType<SceneController>();
        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");
    }

    // Antes del start si por defecto está activo cuando se carga la escena
    private void OnEnable()
    {
        geoLocManager = GeoLocManager.Instance;
        geoLocManager.WhenAreaAvailable += EnableLoadCanvas;
    }

    private void OnDisable()
    {
        geoLocManager.WhenAreaAvailable -= EnableLoadCanvas;
    }

    private void EnableLoadCanvas()
    {
        loadCanvas.SetActive(true);
        notZoneText.SetActive(false);
    }

    // Esto se llama desde el botón para cargar la nueva escena. --> Imita SceneReaction
    public void LoadNewArea()
    {
        string sceneName = geoLocManager.GetCurrentArea();
        string startingPointInLoadedScene = sceneName; // Por claridad

        sceneController.FadeAndLoadScene(geoLocManager.GetCurrentArea());
    }
}
