using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostproEnabler : MonoBehaviour {

    public MonoBehaviour[] postProComponents;

    public void EnablePostproComponents()
    {
        foreach (MonoBehaviour comp in postProComponents)
        {
            comp.enabled = true;
        }
    }

    public void DisablePostproComponents()
    {
        foreach (MonoBehaviour comp in postProComponents)
        {
            comp.enabled = false;
        }
    }

}
