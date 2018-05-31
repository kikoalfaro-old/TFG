using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCanvasManager : MonoBehaviour {

    [Header("Pestañas del menu")]
    public GameObject[] menuTabs;
    [Space]
    public GameObject buttonMenu;
    public GameObject Menu;
    int currentTabIndex;


	// Use this for initialization
	void Start () {
        currentTabIndex = 0;
	}

    public void OpenMenu()
    {
        Menu.SetActive(true);
        menuTabs[0].SetActive(true);
        buttonMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        Menu.SetActive(false);
        menuTabs[currentTabIndex].SetActive(false);
        buttonMenu.SetActive(true);
    }

    public void NextTab()
    {
        menuTabs[currentTabIndex].SetActive(false);
        if (currentTabIndex == menuTabs.Length - 1) currentTabIndex = 0;
        else currentTabIndex ++;
        menuTabs[currentTabIndex].SetActive(true);
    }

    public void PreviousTab()
    {
        menuTabs[currentTabIndex].SetActive(false);
        if (currentTabIndex == 0) currentTabIndex = menuTabs.Length - 1;
        else currentTabIndex --;
        menuTabs[currentTabIndex].SetActive(true);
    }

}
