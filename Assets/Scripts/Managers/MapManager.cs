using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// EL DEFAULT AREA MANAGER Y EL MAP MANAGER PODRÍAN IR PERFECTAMENTE EN EL MISMO SCRIPT...

public class MapManager : MonoBehaviour
{
    private static MapManager instance = null;

    [SerializeField]
    AreaStatusColorDictionary statusColors = new AreaStatusColorDictionary();

    [SerializeField]
    AreaStatusSpriteDictionary statusImage = new AreaStatusSpriteDictionary();

    [Space]
    [SerializeField]
    StringImageDictionary areaImages = new StringImageDictionary();

    GameData gameData;
    GeoLocManager geoLocManager;
    GameManager gameManager;

    bool animationDone; // flag
    bool firstTime;

    [Space]
    [Header("References")]
    public Transform currentPosImg;
    public Image percentageCircle;
    public Text percentageText;
    public Animator percentageTextAnim;
    public Text currentAreaText;
    public GameObject starsPS;

    [Space]
    [Header("FX Parameters")]
    public float timeToSpawnStars;
    public float animationTime;

    public static MapManager Instance
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

    // Esto funciona solo porque en el primer frame (cuando se cogen todas las referencias), el mapa estará desactivado
    private void OnEnable()
    {
        geoLocManager = GeoLocManager.Instance;
        gameManager = GameManager.Instance;

        geoLocManager.OnAreaChanges += ShowCurrentAreaInfo;
        geoLocManager.OnUpdateCoords += SetAreaColors;
        geoLocManager.OnUpdateCoords += ShowPercentageText;
    }


    public void OnDisable()
    {
        geoLocManager.OnAreaChanges -= ShowCurrentAreaInfo;
        geoLocManager.OnUpdateCoords -= SetAreaColors;
        geoLocManager.OnUpdateCoords -= ShowPercentageText;
    }

    private void Awake()
    {
        //Check if instance already exists
        if (Instance == null)

            //if not, set instance to this
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
    }

    private void Start()
    {
        gameData = gameManager.GetGameData();

        if (geoLocManager.GetCurrentArea() != null)
        {
            ShowCurrentAreaInfo();
            SetAreaColors();
            //ShowPercentageText();
        }
        animationDone = false;
        firstTime = false;
    }

    public void DisableMap()
    {
        gameObject.SetActive(false);
    }


    private void SetAreaColors() // Se pueden poner en amarillo si están visitadas pero no completadas
    {
        if (gameData == null) gameData = gameManager.GetGameData();
        if (gameData == null) return;

        foreach (KeyValuePair<string, AreaStatus> area in gameData.areasStatus)
        {
            try
            {
                areaImages[area.Key].color = statusColors[area.Value]; // Devuelve el color que corresponde a ese estado
                areaImages[area.Key].sprite = statusImage[area.Value];
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("No has referenciado el área " + area.Key + " con su imagen en el MapManager");
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("Hay un área que no está en el GeoLocData");
            }
        }
    }

    public void ShowCurrentAreaInfo()
    {
        Debug.Log("Show current area info of map manager");

        string currentArea = geoLocManager.GetCurrentArea().name;

        //Debug.Log("Show current position with current area: " + currentArea + "  Img: " + currentPosImg);

        if (currentArea != GameManager.defaultAreaName) // Si no es el área por defecto, se pone el marcador de posición y el texto del área (o tag)
        {
            currentPosImg.position = new Vector3(areaImages[geoLocManager.GetCurrentArea().name].transform.position.x, areaImages[geoLocManager.GetCurrentArea().name].transform.position.y + 1f, areaImages[geoLocManager.GetCurrentArea().name].transform.position.z);
            currentAreaText.text = currentArea; // <-- AQUÍ SERÍA INTERESANTE METER UNA TAG DESDE LA WEB DE ANGULAR
        }
        else // Si es el área por defecto...
        {
            currentPosImg.position = Vector3.one * 2000f;
            currentAreaText.text = "";
        }
    }


    public void ShowPercentageText()
    {
        if (gameData == null) gameData = gameManager.GetGameData();
        if (gameData == null) return;

        int completedPercentage = gameData.completedPercentage;

        bool isGameCompleted = completedPercentage == 100;
        if (isGameCompleted) Invoke("ActiveStars", timeToSpawnStars);
        percentageTextAnim.SetBool("GameCompleted", isGameCompleted);

        // Ponemos el booleano como que se ha completado el juego para que se ejecute la animación


        // Porcentaje y animación de carga
        percentageText.text = completedPercentage.ToString() + " %";
        //percentageCircle.fillAmount = (float) gameData.completedPercentage / 100f;
        float currentValue = (float)completedPercentage / 100f;
        if (!animationDone) StartCoroutine(LerpPercentage(0, currentValue, animationTime));
        animationDone = true;
    }

    void ActiveStars()
    {
        starsPS.SetActive(true);
    }

    private IEnumerator LerpPercentage(float startValue, float endValue, float time)
    {
        float elapsedTime = 0;
        percentageCircle.fillAmount = startValue;

        while (elapsedTime < time)
        {
            percentageCircle.fillAmount = Mathf.Lerp(percentageCircle.fillAmount, endValue, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //yield return null;
    }
}
