using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/*
 * Corrutina ilumina cada x segundos el cubo que toque del índice generado por el combination manager
 * Después será el turno del input, y así con x niveles
 * 
 * Se puede hacer un sistema de niveles como en el juego de los símbolos, pero de momento simplificado
 */

public class MemoryGameManager : MonoBehaviour
{
    // Parámetros del juego    
    [Header("Game Parameters")]
    public int initialLevel = 2; //Indica los elementos a pulsar seguidos (habrá que hacer el temporizador en función a esto también)
    public int stages; // Número de rondas de juego
    int playedStages;
    int currentLevel;
    int currentIndex = 0; // Índice en el que está comprobando si se ha pulsado (En array "combinación")
    int[] sequence;
    bool[] hitCubes;



    public bool sendFlowchartMessageWhenGameEnds;
    MeshRenderer[] meshRenderers;
    Color originalColor;

    int totalCubes;
    bool canTouch = false;

    [Space]
    [Header("Sequence animation")]
    [SerializeField]
    public Color emmisionColor;
    [SerializeField]
    public Color successColor;
    [SerializeField]
    public Color failureColor;
    public float emissionTime = 1;
    public int timeBetweenCubes = 2; // Intervalo de tiempo entre que un cubo se ilumina y el siguiente
    public int colorMaterialIndex;

    [Space]
    public float punchScaleIntesity;
    public float punchScaleTime;
    WaitForSeconds secondsBetweenCubes;

    AudioSource sound;

    void Awake()
    {
        DOTween.Init();

        // Get references
        secondsBetweenCubes = new WaitForSeconds(timeBetweenCubes);
        sound = GetComponent<AudioSource>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log(meshRenderers[0].materials[colorMaterialIndex].name);
        originalColor = meshRenderers[0].materials[colorMaterialIndex].GetColor("_Color");
        totalCubes = meshRenderers.Length;
    }

    public void StartNewGame()
    {
        ResetGame();
        GenerateNewSequence();
    }

    public void ResetGame()
    {
        currentLevel = initialLevel;
        playedStages = 0;
    }

    bool GameFinished()
    {
        return playedStages == stages;
    }

    void CheckIfGameFinished()
    {
        if (GameFinished()) GetResults();
        else GenerateNewSequence();
    }

    private void GetResults()
    {
        Debug.Log("JUEGO TERMINADO");
        if (sendFlowchartMessageWhenGameEnds) Fungus.Flowchart.BroadcastFungusMessage("GameWon");
    }

    void GenerateNewSequence()
    {
        canTouch = false;
        //REINICIAMOS VARIABLES
        sequence = new int[currentLevel]; //Tendrá que crearse un nuevo array a cada nivel
        hitCubes = new bool[currentLevel];
        currentIndex = 0; //Reinicia el índice

        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = UnityEngine.Random.Range(0, totalCubes);
            //Número random entre 0 y el número total de cubos (indica qué cubos se deben pulsar)                        
        }

        ShowSequence();
        // Mostrar secuencia visualmente con colores y notas

    }

    void ShowSequence()
    {
        canTouch = false;
        Sequence tweenSequence = DOTween.Sequence();
        tweenSequence.OnComplete(EnableTouch);

        foreach (var index in sequence)
        {
            tweenSequence.Append(meshRenderers[index].materials[colorMaterialIndex].DOColor(emmisionColor, emissionTime))
                .Append(meshRenderers[index].materials[colorMaterialIndex].DOColor(originalColor, emissionTime));
        }
    }

    private void EnableTouch()
    {
        Debug.Log("Touch enabled");
        canTouch = true;
    }

    void WellDoneSequence()
    {
        Sequence wellDoneSequence = DOTween.Sequence();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            wellDoneSequence.Join(meshRenderers[i].materials[colorMaterialIndex].DOColor(successColor, emissionTime / 5f))
                        .Append(meshRenderers[i].materials[colorMaterialIndex].DOColor(originalColor, emissionTime /5f));
        }
        wellDoneSequence.OnComplete(CheckIfGameFinished);
    }

    Sequence TouchAnimation(int cubeIndex, Color color)
    {
        Sequence touchAnimationSequence = DOTween.Sequence();
        touchAnimationSequence.Append(meshRenderers[cubeIndex].materials[colorMaterialIndex].DOColor(successColor, emissionTime/5f))
                .Append(meshRenderers[cubeIndex].materials[colorMaterialIndex].DOColor(originalColor, emissionTime/5f));
        return touchAnimationSequence;
    }

    void CheckSequence(int cubeIndex)
    {
        Sequence touchAnimationSequence;
        sound.Play();

        if (cubeIndex == sequence[currentIndex]) //ACIERTA 
        {
            touchAnimationSequence = TouchAnimation(cubeIndex, successColor);

            hitCubes[currentIndex] = true;
            currentIndex++; //Se actualiza el índice

            // Comprueba si ha acabado la partida
            if (currentIndex == currentLevel) //Ha llegado hasta el final del array --> Ha acertado todos en el orden correcto
            {
                canTouch = false;
                Debug.Log("CORRECTO!");
                currentLevel++;
                playedStages++;
                touchAnimationSequence.OnComplete(WellDoneSequence);
            }
        }
        else // FALLA, se reinicia
        {
            touchAnimationSequence = TouchAnimation(cubeIndex, successColor);

            currentIndex = 0; //En el momento en que fallas, se reinicia
            for (int a = 0; a < hitCubes.Length; a++) hitCubes[a] = false;

            touchAnimationSequence.Complete();
            Sequence allRedsSequence = DOTween.Sequence();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                allRedsSequence.Join(meshRenderers[i].materials[colorMaterialIndex].DOColor(failureColor, emissionTime / 5f));
                allRedsSequence.Append(meshRenderers[i].materials[colorMaterialIndex].DOColor(originalColor, emissionTime / 5f));
            }

            allRedsSequence.OnComplete(GenerateNewSequence); //Si falla, se genera una nueva secuencia del mismo nivel
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canTouch)
        {
            int cubeTouchedIndex = GetCubeTouchedIndex();
            if (cubeTouchedIndex != -1)
            {
                Debug.Log("Se ha pulsado el cubo " + cubeTouchedIndex);
                CheckSequence(cubeTouchedIndex);
            }
        }
    }


    int GetCubeTouchedIndex()
    {
        RaycastHit hit = new RaycastHit();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) // En hit se guarda el gameObject con el que ha chocado (Si no choca, Physics.Raycast es false)
        {
            if (hit.collider.tag == "simonCube")
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (meshRenderers[i].gameObject == hit.collider.gameObject)
                    {
                        hit.collider.gameObject.transform.DOPunchScale(Vector3.one * punchScaleIntesity, punchScaleTime, 10, 2);
                        return i;
                    }
                }
            }
        }
        return -1;
    }
}
