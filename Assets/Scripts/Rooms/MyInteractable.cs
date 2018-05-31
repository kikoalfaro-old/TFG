using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MyInteractable : MonoBehaviour {

    Touch touch;
    Vector3 touchPosition;    
    Collider2D myCollider2D;

    public bool hasDialog;

    Item item; //Item oculto en el interactuable.

    private void Start()
    {
        myCollider2D = GetComponent<Collider2D>();
    }


    public void ShowDialog()
    {
        if(hasDialog) Debug.Log("Ahora aparecería el diálogo");
    }


    public bool isTouched(Vector3 touchPosition)
    {
        return myCollider2D.bounds.Contains(touchPosition);
    }

    // ON CLICK


}
