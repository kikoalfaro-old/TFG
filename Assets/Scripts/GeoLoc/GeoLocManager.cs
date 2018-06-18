using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Area
{
    public string name;
    public GeoLocCoordinates centre;
    public double radius;

    public Area(string name, double latitude, double longitude, double radius)
    {
        this.name = name;
        this.centre = new GeoLocCoordinates((float)latitude, (float)longitude);
        this.radius = radius;
    }

    public override string ToString()
    {
        return "Nombre: " + this.name + "\nLatitud: " + this.centre.latitude + "; Longitud: " + this.centre.longitude + "\nRadio: " + this.radius;
    }

    public override bool Equals(object other)
    {
        Area otherArea = (Area)other;
        return this.name == otherArea.name &&
            this.centre.latitude == otherArea.centre.latitude &&
            this.centre.longitude == otherArea.centre.longitude &&
            this.radius == otherArea.radius;
    }

    public override int GetHashCode()
    {
        return this.GetHashCode();
    }
}

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


    // Introducimos las coordenadas como una cadena y hace el split para guardarlo como float
    public GeoLocCoordinates(string coordinates)
    {
        string[] textSplit = coordinates.Split(", "[0]);
        float.TryParse(textSplit[0], out latitude);
        float.TryParse(textSplit[1], out longitude);
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
    string defaultAreaName = GameManager.defaultAreaName;
    Area defaultArea;

    List<Area> allAreas; 
    private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.

    public event Action OnAreaChanges;
    public event Action OnUpdateCoords;

    // debug app
    [Header("Debug References")]
    public Text coordsText;
    public Text zoneText;
    public GameObject VerticalLayout;

    [Space]
    [Header("Current area debug")]
    public string currentCoords_Debug;
    public bool debugMode;

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
    
    public void OnEnable()
    {
        GameManager.Instance.OnDataLoaded += StartGeoLoc;
    }

    public void OnDisable()
    {
        GameManager.Instance.OnDataLoaded -= StartGeoLoc;
    }

    public void StartGeoLoc()
    {
        allAreas = GameManager.Instance.GetAllAreas();
        if (allAreas == null)
            throw new UnityException("Geolocation data could not be found, ensure that it exists in the Persistent scene.");

        sceneController = FindObjectOfType<SceneController>();
        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

        InitialSetup();
        StartCoroutine(UpdateCoordsCoroutine()); // Comienza la corrutina de actualizar la posición cada cierto tiempo

        if (debugMode) Debug.LogWarning("DEBUG MODE IS ENABLED!"); else Debug.LogWarning("DEBUG MODE IS DISABLED (Only for smartphone)");
    }

    void InitialSetup()
    {
        // Valores iniciales por defecto (area Default)
        defaultArea = allAreas[0];
        currentArea = defaultArea;                                                   
        currentCoords = new GeoLocCoordinates();

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
        if (OnUpdateCoords != null) OnUpdateCoords();

        if (!debugMode)
        {
#if UNITY_ANDROID
            currentCoords.latitude = Input.location.lastData.latitude;
            currentCoords.longitude = Input.location.lastData.longitude;
#endif
        } else
        {

            VerticalLayout.SetActive(true);
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
        }

        UpdateArea(); //Si el área actual es 0 (ficticia), no mira si está dentro o no.
        SetTexts();

        Debug.Log("Coords updated");
    }

    void UpdateArea()
    {
        if (currentArea.Equals(defaultArea)) // Sería igual que decir currentArea == allAreas[0] --> Área por defecto
        {
            for (int i = 0; i < allAreas.Count; i++)
            {
                if (allAreas[i].Equals(currentArea)) continue; //Si es la misma, pasa a la siguiente itereación (ya hemos visto que en esta no está)
                // SE SUPONE QUE POR EL COMPARER, LOS OBJETOS SE DEBEN COMPARAR BIEN (Da igual que sean instancias diferentes)

                if (PointInsideArea(currentCoords, allAreas[i]))
                {
                    if (OnAreaChanges != null)
                    {
                        currentArea = allAreas[i]; // ESTO DEBE IR AQUÍ. (Evita que se actualice área sin estar disponible la nueva escena para cargar)
                        OnAreaChanges(); //Llamamos al evento e informamos al DefaultAreaManager que se prepare para cargar la nueva escena
                    }
                        
                    break;
                }
            } //Si llega al final y no la ha actualizado, se quedará en área por defecto            
        }

        // Si no estoy en la escena por defecto pero no estoy dentro del área, cargar la escena por defecto
        else if(!PointInsideArea(currentCoords, currentArea)){
            //sceneController.FadeAndLoadScene(defaultArea.name);            
            currentArea = defaultArea;
            OnAreaChanges();
        }
    }

    private void SetTexts()
    {
        coordsText.text = currentCoords.ToString();
        zoneText.text = currentArea.name;
    }


    #endregion

    /// <summary>
    /// Devuelve si un punto está dentro de un area. Si el parámetro de area es el por defecto, devuelve falso.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    
    bool PointInsideArea(GeoLocCoordinates point, Area area)
    {
        //Debug.Log(point + "inside " + area);
        bool isInsideArea = false;
        if(!area.Equals(defaultArea)) isInsideArea = DistanceBetweenPoints(point, area.centre) <= area.radius; 
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

    public string GetCurrentAreaName()
    {
        return currentArea.name;
    }

    #endregion

}

/*
/// <summary>
/// Comparador entre dos áreas
/// </summary>
public class IdComparer : IEqualityComparer<Area>
{

    public int GetHashCode(Area area)
    {
        if (area == null)
        {
            return 0;
        }
        return area.GetHashCode();
    }

    public bool Equals(Area a1, Area a2)
    {
        if (object.ReferenceEquals(a1, a2))
        {
            return true;
        }
        if (object.ReferenceEquals(a1, null) ||
            object.ReferenceEquals(a2, null))
        {
            return false;
        }
        return a1.name == a2.name &&
            a1.centre.latitude == a2.centre.latitude &&
            a1.centre.longitude == a2.centre.longitude &&
            a1.radius == a2.radius;
    }
}
*/