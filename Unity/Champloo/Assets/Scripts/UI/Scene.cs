using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Scene : NetworkBehaviour {

    [Command]
    private void CmdChangeScene(string scene)
    {
        NetworkServer.SetAllClientsNotReady();
        NetworkManager.singleton.ServerChangeScene(scene);
    }

    public void LoadLevel(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void LoadLevelNetworked(string s)
    {
        //NetworkManager.singleton.ServerChangeScene(s);
        //NetworkServer.SetAllClientsNotReady();
        //LobbyManager.s_Singleton.ServerChangeScene(s);
        //LobbyManager.s_Singleton.CmdChangeScene(s);
        CmdChangeScene(s);
    }

    public void ReturnToLobby()
    {
        LobbyManager.s_Singleton.ServerReturnToLobby();
    }

    public void RestartLevel()
    {
        //NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        LoadLevelNetworked(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

