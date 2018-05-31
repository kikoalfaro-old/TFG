using UnityEngine;
using UnityEngine.UI;

// Asociado al prefab de movimiento (se instanciará con una información de symbol aleatoria)
public class SymbolMovement : MonoBehaviour
{
    Symbol symbol;
    Rigidbody2D _rigidbody;
    bool isInsideRange;
    SymbolsGameManager symbolsGameManager; //Para añadir o quitar de los símbolos que están dentro de un rango
    Image image;
    Color originalColor;

    // Properties:
    public Symbol Symbol
    {
        get
        {
            return symbol;
        }

        set
        {
            symbol = value;
            GetComponent<Image>().sprite = value.symbolSprite; // Le ponemos el sprite correspondiente a ese símbolo
        }
    }
    public Rigidbody2D Rigidbody
    {
        get
        {
            return GetComponent<Rigidbody2D>(); //Solo se accede una vez, así que sin problema
        }

        set
        {
            _rigidbody = value;
        }
    }
    public bool IsInsideRange
    {
        get
        {
            return isInsideRange;
        }

        set
        {
            isInsideRange = value;
        }
    }

    private void Start()
    {
        symbolsGameManager = GetComponentInParent<SymbolsGameManager>();
        image = GetComponent<Image>();
        originalColor = image.color;
    }

    // Entra al rango
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Range"))
        {
            //Debug.Log("INSIDE RANGE!");
            symbolsGameManager.AddAvailableSymbol(this);
            isInsideRange = true;
            //image.color = Color.white; // Highlight --> Feedback visual (se podría parametrizar)
            Color highlightColor;
            ColorUtility.TryParseHtmlString("#F2EEB3FF", out highlightColor);
            image.color = highlightColor;
        }
    }

    // Sale del rango
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Range") && isInsideRange) //Evita errores
        {
            //Debug.Log("OUTSIDE RANGE!");
            symbolsGameManager.RemoveAvailableSymbol(this);
            isInsideRange = false;
            image.color = originalColor;
        }
    }

}
