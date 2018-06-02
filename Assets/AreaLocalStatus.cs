using UnityEngine;
using Fungus;
using System;

public class AreaLocalStatus : MonoBehaviour
{   
    /*
    public void BroadcastStatusMessage()
    {
        string msg = GameManager.Instance.GetCurrentAreaStatus().ToString();
        Debug.Log("BROADCASTED: " + msg);
        Flowchart.BroadcastFungusMessage(msg);
    }
    */

    public string GetCurrentStatus()
    {
        return GameManager.Instance.GetCurrentAreaStatus().ToString();
    }

    public string UpdateCurrentStatus(AreaStatus newStatus)
    {
        GameManager.Instance.UpdateCurrentAreaStatus(newStatus); //Lo actualizo en el gameManager
        return newStatus.ToString();
    }
}
