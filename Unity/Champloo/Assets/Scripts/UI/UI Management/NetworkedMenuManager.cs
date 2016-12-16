using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkedMenuManager : NetworkBehaviour {

    [SerializeField]
    private Menu CurrentMenu;

    private Menu startMenu;

    private bool firstFrame = true;

    private void Awake()
    {
        startMenu = CurrentMenu;
    }

    private void LateUpdate()
    {
        if (firstFrame)
        {
            firstFrame = false;
            LoadDefaultMenu();
        }
    }

    private void OnEnable()
    {
        GoToMenu(startMenu);
    }

    private void LoadDefaultMenu()
    {
        ShowMenu(CurrentMenu);
    }

    public void ShowMenu(Menu menu)
    {
        CmdUpdateMenu(menu.name);
    }

    private void GoToMenu(Menu menu)
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

    [Command]
    private void CmdUpdateMenu(string menuObjectName)
    {
        RpcUpdateMenu(menuObjectName);
    }

    [ClientRpc]
    private void RpcUpdateMenu(string menuObjectName)
    {
        Menu[] menus = GameObject.FindObjectsOfType<Menu>();
        bool found = false;
        for (int i =0; i < menus.Length; i++)
        {
            if(menus[i].name == menuObjectName)
            {
                GoToMenu(menus[i]);
                found = true;
            }
        }
        if(!found)
        {
            Debug.LogError("Could not find menu with name " + menuObjectName);
        }
    }
}
