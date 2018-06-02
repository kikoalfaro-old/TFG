using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class GeoLocCoordinates
{
    public float latitude; // x (horizontal)
    public float longitude; // y (vertical)

    public GeoLocCoordinates() //Default
    {
        this.latitude = 0f;
        this.longitude = 0f;
    }

    public GeoLocCoordinates(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }
    public override string ToString()
    {
        return latitude + ", " + longitude;
    }
}

public class GeoLocManager : MonoBehaviour
{
    private static GeoLocManager instance = null;

    GeoLocCoordinates currentCoords; // Coordenadas espaciales actuales
    Area currentArea; // Área en la que se encuentra el jugador. Nulo si está fuera de los límites del juego.

    public float refreshTime = 2f; // Tiempo de actualización de las coordenadas (para que no consuma tanta batería...) --> De momento, por defecto a 20 segs
    public string defaultSceneName = "Default";
    Area[] areas; // Todas las áreas del juego con su respectiva info.

    GeoLocData geoLocData; //Singleton
    private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.

    public event Action WhenSceneAvailable;

    // debug app
    [Header("Debug References")]
    public Text coordsText;
    public Text zoneText;

    [Space]
    [Header("Current area debug")]
    public string currentCoords_Debug;

    public static GeoLocManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        geoLocData = GeoLocData.Instance;
        if (!geoLocData)
            throw new UnityException("Geolocation data could not be found, ensure that it exists in the Persistent scene.");

        sceneController = FindObjectOfType<SceneController>();
        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

        InitialSetup();
        StartCoroutine(UpdateCoordsCoroutine()); // Comienza la corrutina de actualizar la posición cada cierto tiempo
    }

    void InitialSetup()
    {
        // Valores iniciales por defecto
        currentCoords = new GeoLocCoordinates();
        currentArea = geoLocData.allAreas[0]; //Empieza siendo la zona por defecto

        Input.location.Start();
        UpdateCoods(); //Pone las coordenadas actuales al inicializarse y sitúa el juego
    }

    #region Updaters

    IEnumerator UpdateCoordsCoroutine()
    {
        while (true)
        {
            UpdateCoods();
            yield return new WaitForSeconds(refreshTime);
        }
    }


    void UpdateCoods()
    {

#if UNITY_ANDROID
        currentCoords.latitude = Input.location.lastData.latitude;
        currentCoords.longitude = Input.location.lastData.longitude;
#endif

#if UNITY_EDITOR
        // Poniendo el punto como si estuviéramos justo en el centro de la zona (muy improbable...)
        try
        {
            string[] textSplit = currentCoords_Debug.Split(", "[0]);
            float.TryParse(textSplit[0], out currentCoords.latitude);
            float.TryParse(textSplit[1], out currentCoords.longitude);
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Log("La zona introducida no está dentro del rango de zonas creado.");
        }
#endif

        UpdateArea(); //Si el área actual es 0 (ficticia), no mira si está dentro o no.
        SetTexts();
    }

    void UpdateArea()
    {
        if (currentArea.id == 0)
        {
            foreach (var zone in geoLocData.allAreas)
            {
                if (zone == currentArea) continue; //Si es la misma, pasa a la siguiente itereación (ya hemos visto que en esta no está)

                if (PointInsideArea(currentCoords, zone))
                {
                    if (WhenSceneAvailable != null)
                    {
                        currentArea = zone; // ESTO DEBE IR AQUÍ. (Evita que se actualice área sin estar disponible la nueva escena para cargar)
                        WhenSceneAvailable(); //Llamamos al evento e informamos al DefaultAreaManager que se prepare para cargar la nueva escena
                    }
                        
                    break;
                }
            } //Si llega al final y no la ha actualizado, se quedará en zona 0            
        }

        // Si no estoy en la escena por defecto pero no estoy dentro del área, cargar la escena por defecto
        else if(!PointInsideArea(currentCoords, currentArea)){
            sceneController.FadeAndLoadScene(defaultSceneName);
            currentArea = geoLocData.allAreas[0];
        }

        // Otra idea, para grafo de áreas...
        // Buscar en las áreas adyacentes y ver en cuál está (de momento, después se puede optimizar) 
    }

    private void SetTexts()
    {
        coordsText.text = currentCoords.ToString();
        zoneText.text = currentArea.id.ToString();
    }


    #endregion

    /// <summary>
    /// Devuelve si un punto está dentro de un area. Si el parámetro de zona es el por defecto, devuelve falso.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    //OJO:  Aquí hemos de CAMBIARLO y pasar como parámetro un objeto de tipo AREA... (también vale para elementos)
    bool PointInsideArea(GeoLocCoordinates point, Area area)
    {
        bool isInsideArea = false;
        if(area.id != 0) isInsideArea = DistanceBetweenPoints(point, area.centre) <= GeoLocData.Instance.areaRadius; 
        // Un punto está dentro de un círculo si la distancia desde él hasta el centro es menor o igual que r (radio)

        return isInsideArea;
    }

    #region Math

    public float DistanceBetweenPoints(GeoLocCoordinates pointA, GeoLocCoordinates pointB)
    {
        var d1 = pointA.latitude * (Math.PI / 180.0);
        var num1 = pointA.longitude * (Math.PI / 180.0);
        var d2 = pointB.latitude * (Math.PI / 180.0);
        var num2 = pointB.longitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        double doubleDistance = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));

        float distance = (float)doubleDistance;
        //Debug.Log("La distancia entre el punto " + pointA + " y el punto " + pointB + " es: " + distance);
        return distance;
    }


    // ESTA VERSIÓN VIEJA DE DISTANCIA ENTRE PUNTOS NO FUNCIONA. QUITARLA UNA VEZ HECHO EL DEBUG
    /*
    public float DistanceBetweenPoints(GeoLocCoordinates pointA, GeoLocCoordinates pointB)
    {
        double distance;
        var R = 6378.137; // Radius of earth in KM

        var dLat = pointB.latitude * Mathf.PI / 180 - pointA.latitude * Mathf.PI / 180;
        var dLon = pointB.longitude * Mathf.PI / 180 - pointB.longitude * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
          Mathf.Cos(pointA.latitude * Mathf.PI / 180) * Mathf.Cos(pointB.latitude * Mathf.PI / 180) *
          Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        distance = R * c;

        distance = distance * 1000f; // meters
                                     //convert distance from double to float
        float distanceFloat = (float)distance;
        return distanceFloat;
    }
    */

    public float EuclideanDistanceBetweenPoints(GeoLocCoordinates pointA, GeoLocCoordinates pointB)
    {
        float Ax = pointA.latitude;
        float Ay = pointA.longitude;
        float Bx = pointB.latitude;
        float By = pointB.longitude;

        return (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(Ax - Bx), 2) + Mathf.Pow(Mathf.Abs(Ay - By), 2)));
    }
    #endregion
    

    #region Getters and setters
    public GeoLocCoordinates GetCurrentCoords()
    {
        return currentCoords;
    }

    public Area GetCurrentArea()
    {
        return currentArea;
    }

    #endregion


}
