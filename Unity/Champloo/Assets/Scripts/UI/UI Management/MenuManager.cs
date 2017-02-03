using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Menu CurrentMenu;

    private bool firstFrame = true;

    private void LateUpdate()
    {
        if (firstFrame)
        {
            firstFrame = false;
            LoadDefaultMenu();
        }
    }

    private void LoadDefaultMenu()
    {
        ShowMenu(CurrentMenu);
    }
    /*
    public void Start()
    {
    }
    */
    public void ShowMenu(Menu menu)
    {

        if (CurrentMenu != null)
        {
            CurrentMenu.IsOpen = false;
        }

        CurrentMenu = menu;

        if (menu != null)
        {
            CurrentMenu.IsOpen = true;
        }
    }
}
