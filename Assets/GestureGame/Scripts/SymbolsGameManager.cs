using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using System;
using UnityEngine.UI;
using Fungus;

public class SymbolsGameManager : MonoBehaviour /*, IPointerDownHandler */
{

    public Difficulty difficulty; //Aquí selecciono la dificultad que quiero en este nivel concreto
    
    // SCORE REFERENCES
    int score; // Puntuación total
    Text scoreText;
    public Text newScoreText;
    Animator newScoreAnimator;

    WaitForSeconds timeBetweenSymbols;
    int spawnedSymbols; // Cantidad de símbolos spawneados en este momento
    int succededSymbols;
    Level level; // Info del nivel actual

    List<SymbolMovement> symbolsInsideRange; //Símbolos que están dentro de algún rango
    AudioSource successAudio;

    public bool sendFlowchartMessageWhenGameEnds;

    private void Start()
    {
        LevelSetup();
        StartCoroutine("SpawnSymbol");
    }

    private void LevelSetup()
    {
        // Reinicio variables
        spawnedSymbols = 0;
        succededSymbols = 0;
        score = 0;

        level = SymbolsGameInfo.GetLevel(difficulty); //Cojo la info del nivel de dificultad que quiero
        timeBetweenSymbols = new WaitForSeconds(level.timeBetweenSymbols);
        symbolsInsideRange = new List<SymbolMovement>();

        scoreText = GetComponentInChildren<Text>();
        newScoreAnimator = scoreText.GetComponentInChildren<Animator>();

        successAudio = GetComponent<AudioSource>();
    }

    // Aparecer símbolo cada x tiempo
    IEnumerator SpawnSymbol()
    {
        while (spawnedSymbols < level.amountOfSymbolsToSpawn)
        {
            GenerateSymbol();
            spawnedSymbols++;
            yield return timeBetweenSymbols;
        }

        yield return new WaitForSeconds(2f); //Espero 2 segs a mostrar resultados

        GetResults();
        // Fin del juego con resultados y fin de la corrutina.
        yield return null;
    }

    private void GetResults() // Aquí iría el final de la reacción
    {
        Debug.Log("Porcentaje de acierto: " + GetTotalAccuracy() + "%");
        if (GetTotalAccuracy() >= level.minTotalAccuracy)
        {
            Debug.Log("Nivel superado!");
            if (sendFlowchartMessageWhenGameEnds) Fungus.Flowchart.BroadcastFungusMessage("GameWon");
        }
        else
        {
            Debug.Log("Nivel no superado...");
            if (sendFlowchartMessageWhenGameEnds) Fungus.Flowchart.BroadcastFungusMessage("GameLost");
        }
    }

    void GenerateSymbol()
    {
        int spawningPoint_Index = UnityEngine.Random.Range(0, level.spawningPoints.Length); //Ojo, primero inclusivo y segundo exclusivo
        RectTransform spawningPoint = level.spawningPoints[spawningPoint_Index].transform;
        Vector2 direction = level.spawningPoints[spawningPoint_Index].Direction;
        float symbolSpeed = UnityEngine.Random.Range(level.symbolMinSpeed, level.symbolMaxSpeed + 1);
        SymbolMovement symbolMov = level.symbolMov;

        // Instantiate
        SymbolMovement newSymbolMov;
        newSymbolMov = Instantiate(symbolMov, spawningPoint.localPosition, transform.rotation) as SymbolMovement;
        // Si encuentro una forma mejor de hacerlo, lo haré... Spaguetti intensifies!
        newSymbolMov.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(direction * symbolSpeed); // Asigno la velocidad lineal (De momento fija, pero haremos un random ahora)
        newSymbolMov.Symbol = AllGestures.GetRandomSymbol(); //Se le asigna uno de los símbolos disponibles aleatoriamente (la imagen se asigna sola)
        newSymbolMov.transform.SetParent(this.transform, false);
    }

    public void OnSymbolDrawn (Result result) //Evento llamado cuando se suelta el dedo
    {
        CheckSymbols(result.GestureClass);
    }

    /// <summary>
    /// Comprueba si el símbolo dado está dentro de algún rango de la partida
    /// </summary>
    /// <param name="symbolName"></param>
    private void CheckSymbols(string symbolName)  // Si está dentro del rango y se realiza correctamente...
    {
        foreach (var symbolMov in symbolsInsideRange)
        {
            if(symbolName == symbolMov.Symbol.symbolName) // Acertado --> Coinciden las etiquetas
            {
                symbolMov.IsInsideRange = false;
                symbolsInsideRange.Remove(symbolMov);
                UpdateScore(symbolMov); //Se actualiza la puntuación final
                successAudio.Play(); //Suena el sonido de acertado
                Destroy(symbolMov.gameObject);
                succededSymbols++;
                //Debug.Log("Símbolo acertado!");
                break; //Rompo para no alterar la lista en tiempo de ejecución (error) 
           }                      
        }
    }

    private void UpdateScore(SymbolMovement symbol)
    {
        // Puntuación = puntos fijos por acierto + velocidad * 5 - tamaño rangos / 3
        float velocity = symbol.Rigidbody.velocity.magnitude;
        float rangeScale = level.ranges[0].transform.localScale.x;

        int obtainedScore = Mathf.RoundToInt(150 + velocity * 5 - rangeScale/3);

        // Sale la nueva puntuación (trigger a animator), y cuando acaba la animación se actualiza.
        newScoreText.text = "+ " + obtainedScore;
        newScoreAnimator.SetTrigger("Scored");


        score += obtainedScore; // Actualizo puntuación

        scoreText.text = score.ToString();
    }

    #region Range Management
    public void AddAvailableSymbol(SymbolMovement symbol)
    {
        symbolsInsideRange.Add(symbol);
    }

    public void RemoveAvailableSymbol(SymbolMovement symbol)
    {
        symbolsInsideRange.Remove(symbol);
    }

    public void DestroySymbol(SymbolMovement symbol)
    {
        symbolsInsideRange.Remove(symbol);
        Destroy(symbol);
    }
    #endregion


    float GetTotalAccuracy() //Devuelve porcentaje de acierto (en %). (Depende del nivel, tendrá que llegar a un porcentaje determinado)
    {
        return ((float)succededSymbols / (float)spawnedSymbols) * 100;
    }

    /*
* LA DIFICULTAD DEPENDE DE:
* - Velocidad de los símbolos (= de aleatoria siempre, pero mayor rango)
* - Dirección de los símbolos
* - Intervalo de aparición
* - Conjunto de símbolos usados
*/

    // De momento usaré solo un tipo de símbolo (cuadrado)

    // SE ELEGIRÁ UN PUNTO ALEATORIO Y A PARTIR DE ÉL YA SE CALCULA LA DIRECCIÓN A LA QUE VA Y LA VELOCIDAD "ALEATORIA" QUE TOMARÁ (DEPENDE DE LA DIFICULTAD)
    // A RAÍZ DE ESTO, TAMIBÉN VARIARÁ EL CUADRO DE ACCIÓN (Cuando está verde, se puede actuar)


}

