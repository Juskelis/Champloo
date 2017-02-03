using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkedMenuManager : NetworkBehaviour {

    [SerializeField]
    private MenuManager target;

    public void ShowMenu(Menu menu)
    {
        if(hasAuthority) CmdUpdateMenu(menu.name);
    }
    
    private void GoToMenu(Menu menu)
    {
        target.ShowMenu(menu);
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
        foreach (Menu m in menus)
        {
            if(m.name == menuObjectName)
            {
                GoToMenu(m);
                found = true;
            }
        }
        if(!found)
        {
            Debug.LogError("Could not find menu with name " + menuObjectName);
        }
    }
}
