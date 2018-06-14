using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [Space]
    public Transform currentPosImg;
    public Image percentageCircle;
    public float animationTime;
    public Text percentageText;

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
        geoLocManager.WhenAreaAvailable += ShowCurrentPosition;
        geoLocManager.WhenAreaAvailable += ShowVisualInformation;
        GameManager.Instance.OnDataLoaded += ShowVisualInformation;
    }

    public void OnDisable()
    {
        GameManager.Instance.OnDataLoaded -= ShowVisualInformation;
        geoLocManager.WhenAreaAvailable -= ShowVisualInformation;
        geoLocManager.WhenAreaAvailable -= ShowCurrentPosition;
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

    public void ShowVisualInformation()
    {
        gameData = GameManager.Instance.GetGameData();
        SetAreaColors();
        ShowPercentage();
    }

    public void DisableMap()
    {
        gameObject.SetActive(false);
    }

    private void SetAreaColors() // Se pueden poner en amarillo si están visitadas pero no completadas
    {
        if(gameData == null) gameData = GameManager.Instance.GetGameData(); // FATAL.
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

    private void ShowCurrentPosition()
    {        
        Area currentArea = geoLocManager.GetCurrentArea();
        Debug.Log("Show current position with current area: " + currentArea + "  Img: " + currentPosImg);
        if (currentArea.name != GameManager.defaultAreaName) currentPosImg.position = new Vector3(areaImages[geoLocManager.GetCurrentArea().name].transform.position.x, areaImages[geoLocManager.GetCurrentArea().name].transform.position.y + 1f, areaImages[geoLocManager.GetCurrentArea().name].transform.position.z);
    }

    private void ShowPercentage()
    {
        percentageText.text = gameData.completedPercentage.ToString() + " %";
        //percentageCircle.fillAmount = (float) gameData.completedPercentage / 100f;
        float currentValue = (float)gameData.completedPercentage / 100f;
        StartCoroutine(LerpPercentage(0, currentValue, animationTime));
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
