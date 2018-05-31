using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;


// Guardo esta clase a modo de utilidad para futuros casos
[RequireComponent(typeof(BoxCollider))]
public class FungusCollisionTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Trigger!");
            Flowchart.BroadcastFungusMessage(transform.name);
        }
    }

}
