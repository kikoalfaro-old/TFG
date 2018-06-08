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

    public Camera UICamera;
    public Camera ARCamera;

    public string GetCurrentStatus()
    {
        return GameManager.Instance.GetCurrentAreaStatus().ToString();
    }

    public string UpdateCurrentStatus(AreaStatus newStatus)
    {
        GameManager.Instance.UpdateCurrentAreaStatus(newStatus); //Lo actualizo en el gameManager
        return newStatus.ToString();
    }

    public void ClearCurrentFlowchart()
    {
        //GameObject sayDialog = FindObjectOfType<SayDialog>().gameObject;
        GameObject sayDialog = GameObject.Find("SayDialog");

        if (sayDialog != null)
        {
            Debug.Log("sayDialog found and destroyed");
            Destroy(sayDialog);
        }

        //GameObject menuDialog = FindObjectOfType<MenuDialog>().gameObject;
        GameObject menuDialog = GameObject.Find("MenuDialog");

        if (menuDialog != null)
        {
            Debug.Log("menuDialog found and destroyed");
            Destroy(menuDialog);
        }
    }

    public void DisconnectButton()
    {
        Flowchart.BroadcastFungusMessage("Disconnect");
        FindObjectOfType<SceneReaction>().ChangeScene();
    }

    public void CloseInfoButton()
    {
        Flowchart.BroadcastFungusMessage("CloseInfo");
    }
}
