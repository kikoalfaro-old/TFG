using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using PDollarGestureRecognizer;

public class GestureRecognizer : MonoBehaviour
{
    public Camera drawingCamera;
    SymbolsGameManager symbolsGameManager;
    public Transform gestureTrailPrefab;

    [Tooltip("Tolerancia (0 a 1) para el reconocimiento de los gestos")]
    public float minScore = 0.7f;

    private List<Gesture> trainingSet = new List<Gesture>();
    private List<Point> points = new List<Point>();

    private Vector3 virtualKeyPosition = Vector2.zero;

    private RuntimePlatform platform;

    private Transform currentTrailRenderer;
    private float startDrawingTime;

    //GUI
    private string message;
    private bool recognized;
    private string newGestureName = "";

    void Start()
    {
        symbolsGameManager = GetComponent<SymbolsGameManager>();
        platform = Application.platform;

        //Load pre-made gestures
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));


        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));

    }

    void Update()
    {

        if (currentTrailRenderer != null) Debug.Log(currentTrailRenderer.position);
        #region Check platform
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }
        }
        #endregion

        // Si se pulsa por primera vez, instanciar el TrailRenderer
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        // Si se está pulsando en el área de dibujado, sigue añadiendo puntos a la línea y la va dibujando
        if (Input.GetMouseButton(0))
        {
            KeepDrawing();
        }

        // Si se ha levantado el dedo, se ha dibujado un gesto y todavía no ha sido reconocido --> RECONOCERLO
        if (Input.GetMouseButtonUp(0) && !recognized)
        {
            Recognize();
            ClearLine();
        }

    }
    
    private void StartDrawing()
    {
        RaycastHit hit;
        var ray = drawingCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "GesturePlane")
        {
            currentTrailRenderer = Instantiate(gestureTrailPrefab, hit.point, Quaternion.identity) as Transform;
            startDrawingTime = Time.unscaledTime;
        }
    }

    private void KeepDrawing()
    {
        RaycastHit hit;
        var ray = drawingCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "GesturePlane")
        {
            points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, 1));
            currentTrailRenderer.position = hit.point;
        }
    }

    private void ClearLine()
    {
        if (recognized) // Flag
        { //Si se ha reconocido, quita todas las lineRenderer de la pantalla y vuelve a reiniciar el contador
            recognized = false;
            points.Clear();
            Destroy(currentTrailRenderer.gameObject);
        }
    }

    void Recognize()
    {
        recognized = true;
        if (Time.unscaledTime - startDrawingTime < 0.5f) return;
        
        Gesture candidate = new Gesture(points.ToArray());
        try
        {
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
            message = gestureResult.GestureClass + " " + gestureResult.Score;

            // Comprobar si se ha acertado
            symbolsGameManager.OnSymbolDrawn(gestureResult);
        }
        catch (IndexOutOfRangeException exception)
        {
            Debug.LogException(exception, this);
        }


        //if (IsGestureCorrect(gestureResult.Score)) //Se puede hacer un score para todos o que cada uno tenga su nivel de score (GestureClass). De momento, todos igual
        //    Debug.Log("¡Correcto!");
        //else Debug.Log("ERROR!");
    }

    private bool IsGestureCorrect(float score)
    {
        return score >= minScore;
    }


    // Esto lo dejamos aquí para poder añadir gestos personalizados.

    //    void OnGUI()
    //    {

    //        //GUI.Box(drawArea, "Draw Area");

    //        //GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

    //        /*
    //        if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
    //        {

    //            Recognize();
    //        }
    //        */

    //        /*
    //        GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
    //        newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);
    //        */

    //        if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
    //        {

    //            string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

    //#if !UNITY_WEBPLAYER
    //            GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
    //#endif

    //            trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

    //            newGestureName = "";
    //        }
    //    }

}
