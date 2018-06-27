using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// EL DEFAULT AREA MANAGER Y EL MAP MANAGER PODRÍAN IR PERFECTAMENTE EN EL MISMO SCRIPT...
public class DefaultAreaManager : MonoBehaviour {

    public GameObject loadCanvas;
    public GameObject notAreaText;
    public GameObject lockedAreaText;
    private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.
    private GeoLocManager geoLocManager;

    // Use this for initialization
    void Start () {

        sceneController = FindObjectOfType<SceneController>();
        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

        if (geoLocManager.GetCurrentArea() != null)  SetLoadCanvas();

        GPGSManager.Instance.ShowButtons();
    }

    // Antes del start si por defecto está activo cuando se carga la escena
    private void OnEnable()
    {
        geoLocManager = GeoLocManager.Instance;
        geoLocManager.OnAreaChanges += SetLoadCanvas;
    }

    private void OnDisable()
    {
        geoLocManager.OnAreaChanges -= SetLoadCanvas;
    }

    private void SetLoadCanvas()
    {
        Debug.Log("When area changes, set load canvas ");
        Area currentArea = geoLocManager.GetCurrentArea();
        if (currentArea.name != GameManager.defaultAreaName)
        {
            if (!GameManager.Instance.IsAreaLocked(currentArea)) // Si no está bloqueada, que salga el botón de entrar en el área
            {
                lockedAreaText.SetActive(false);
                loadCanvas.SetActive(true);
                notAreaText.SetActive(false);
            }
            else // Si está bloqueada, que salga otro texto informando
            {
                lockedAreaText.SetActive(true);
                loadCanvas.SetActive(false);
                notAreaText.SetActive(false);
            }

        } else
        {
            lockedAreaText.SetActive(false);
            loadCanvas.SetActive(false);
            notAreaText.SetActive(true);
        }
    }

    // Esto se llama desde el botón para cargar la nueva escena. --> Imita SceneReaction
    public void LoadNewArea()
    {
        GPGSManager.Instance.HideButtons();
        string sceneName = geoLocManager.GetCurrentArea().name;
        string startingPointInLoadedScene = sceneName; // Por claridad

        sceneController.FadeAndLoadScene(geoLocManager.GetCurrentArea().name);
    }
}