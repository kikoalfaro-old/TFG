using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TouchAndTween : MonoBehaviour
{
    // Movement parameters
    [Header("Movement speed")]
    public float moveSpeed = 2f;
    float moveDuration;
    float distance;
    Vector3 touchPosition, whereToMove;

    public GameObject touchEffect;

    Rigidbody2D rb;
    Touch touch;

    [Header("Margin offset with interactuable objects")]
    public float moveOffset = 3f; // Distancia de separación cuando se mueve hacia interactuables

    // Interactuables de la escena
    MyInteractable[] interactables; //Array de objetos interactuables que hay en el nivel
    int currentTouched = -1; // Índice del interactuable tocado

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        interactables = FindObjectsOfType<MyInteractable>();
        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                GameObject.Instantiate(touchEffect, touchPosition, Quaternion.identity);

                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0f;
                distance = Math.Abs(touchPosition.x - transform.position.x);
                moveDuration = distance / moveSpeed;

                StartCoroutine(Move());
            }
        }
    }

    // Corrutina de movimiento con DoTween
    IEnumerator Move()
    {
        if (InteractableTouched())
        {
            Debug.Log("Interactable touched!");
            rb.DOMoveX(touchPosition.x - moveOffset, moveDuration);
            interactables[currentTouched].ShowDialog();
            currentTouched = -1; // Vuelve a ponerlo por defecto
        }
        else rb.DOMoveX(touchPosition.x, moveDuration);

        yield return null;
    }


    /// <summary>
    /// Comprueba si algún interactuable del nivel ha sido tocado
    /// </summary>
    /// <returns></returns>
    bool InteractableTouched()
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].isTouched(touchPosition))
            {
                currentTouched = i;
                return true;
            }
        }

        return false;
    }
}