using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Provides a networked MenuManager solution by wrapping local MenuManagers in a networked call
/// </summary>
public class NetworkedMenuManager : NetworkBehaviour {

    [SerializeField]
    private MenuManager target;

    public void ShowMenu(Menu menu)
    {
        if (hasAuthority)
        {
            CmdUpdateMenu(menu != null?menu.name:null);
        }
    }
    
    /// <see cref="MenuManager"/>
    public void ClearMenu()
    {
        ShowMenu(null);
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
        if (string.IsNullOrEmpty(menuObjectName))
        {
            target.ClearMenu();
            return;
        }

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
