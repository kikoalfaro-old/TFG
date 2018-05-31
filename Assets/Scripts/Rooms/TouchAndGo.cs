using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchAndGo : MonoBehaviour
{

    [SerializeField]
    float moveSpeed = 5f;

    public GameObject touchEffect;

    Rigidbody2D rb;

    Touch touch;
    Vector3 touchPosition, whereToMove;
    bool isMoving = false;
    float initialY;
    float previousDistanceToTouchPos, currentDistanceToTouchPos;

    [Tooltip("Distancia de margen con interactuables")]
    public float moveOffset; // Distancia de separación cuando se mueve hacia interactuables

    MyInteractable[] interactables; //Array de objetos interactuables que hay en el nivel
    int currentTouched = -1; // Índice del interactuable tocado

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        interactables = FindObjectsOfType<MyInteractable>();
        initialY = transform.position.y;
    }

    void Update()
    {
        if (isMoving)
            currentDistanceToTouchPos = (touchPosition - transform.position).magnitude; //módulo del Vector 2D

        if (Input.touchCount > 0)
        {          
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("TouchNGo");
                Move();
            }
        }

        // Ha llegado a su destino
        if (currentDistanceToTouchPos > previousDistanceToTouchPos)
        {
            isMoving = false;
            rb.velocity = Vector2.zero;
            if(currentTouched != -1)
            {
                interactables[currentTouched].ShowDialog();
                currentTouched = -1; // Vuelve a ponerlo por defecto
            }
                    
        }

        if (isMoving)
            previousDistanceToTouchPos = (touchPosition - transform.position).magnitude;
    }

    void Move()
    {
        previousDistanceToTouchPos = 0;
        currentDistanceToTouchPos = 0;
        isMoving = true;
        touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = 0;
        GameObject.Instantiate(touchEffect, touchPosition, Quaternion.identity);
        touchPosition.y = initialY;

        if (InteractableTouched())
        {
            whereToMove = (touchPosition - transform.position).normalized;
            // Aplico distancia de márgen
        }
        else
        {
            whereToMove = (touchPosition - transform.position).normalized;
        }


        rb.velocity = new Vector2(whereToMove.x * moveSpeed, whereToMove.y * moveSpeed);
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
