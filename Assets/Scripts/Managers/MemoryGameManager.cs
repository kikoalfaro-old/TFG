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
    public bool sendFlowchartMessageWhenGameEnds;
    public MeshRenderer[] cubes;
    Color originalColor;

    int totalCubes;
    bool canTouch = false;

    [Header("Sequence animation")]
    public float emissionTime = 1;
    public int timeBetweenCubes = 2; // Intervalo de tiempo entre que un cubo se ilumina y el siguiente
    WaitForSeconds secondsBetweenCubes;

    // Parámetros del juego
    [Space]
    public int initialLevel = 2; //Indica los elementos a pulsar seguidos (habrá que hacer el temporizador en función a esto también)
    public int stages; // Número de rondas de juego
    int playedStages;
    int currentLevel;
    int currentIndex = 0; // Índice en el que está comprobando si se ha pulsado (En array "combinación")
    int[] sequence;
    bool[] hitCubes;

    AudioSource sound;

    void Awake()
    {
        DOTween.Init();

        // Get references
        secondsBetweenCubes = new WaitForSeconds(timeBetweenCubes);
        sound = GetComponent<AudioSource>();
        //cubes = GetComponentsInChildren<MeshRenderer>();
        originalColor = cubes[0].material.color;
        totalCubes = cubes.Length;
    }

    private void OnEnable()
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
            tweenSequence.Append(cubes[index].material.DOColor(Color.red, emissionTime))
                .Append(cubes[index].material.DOColor(originalColor, emissionTime));
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
        for (int i = 0; i < cubes.Length; i++)
        {
            wellDoneSequence.Join(cubes[i].material.DOColor(Color.cyan, 0.05f))
                        .Join(cubes[i].material.DOColor(originalColor, 1f));
        }
        wellDoneSequence.OnComplete(CheckIfGameFinished);
    }

    Sequence TouchAnimation(int cubeIndex, Color color)
    {
        Sequence touchAnimationSequence = DOTween.Sequence();
        touchAnimationSequence.Append(cubes[cubeIndex].material.DOColor(Color.cyan, emissionTime))
                .Append(cubes[cubeIndex].material.DOColor(originalColor, emissionTime));
        return touchAnimationSequence;
    }

    void CheckSequence(int cubeIndex)
    {
        Sequence touchAnimationSequence;
        sound.Play();

        if (cubeIndex == sequence[currentIndex]) //ACIERTA 
        {
            touchAnimationSequence = TouchAnimation(cubeIndex, Color.cyan);

            hitCubes[currentIndex] = true;
            currentIndex++; //Se actualiza el índice

            // Comprueba si ha acabado la partida
            if (currentIndex == currentLevel) //Ha llegado hasta el final del array --> Ha acertado todos en el orden correcto
            {
                Debug.Log("CORRECTO!");
                currentLevel++;
                playedStages++;
                touchAnimationSequence.OnComplete(WellDoneSequence);
            }
        }
        else // FALLA, se reinicia
        {
            touchAnimationSequence = TouchAnimation(cubeIndex, Color.cyan);

            currentIndex = 0; //En el momento en que fallas, se reinicia
            for (int a = 0; a < hitCubes.Length; a++) hitCubes[a] = false;

            touchAnimationSequence.Complete();
            Sequence allRedsSequence = DOTween.Sequence();
            for (int i = 0; i < cubes.Length; i++)
            {
                allRedsSequence.Join(cubes[i].material.DOColor(Color.red, 0.05f))
                            .Join(cubes[i].material.DOColor(originalColor, 1f));
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
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (cubes[i].gameObject == hit.collider.gameObject) return i;
                }
            }
        }
        return -1;
    }



}
