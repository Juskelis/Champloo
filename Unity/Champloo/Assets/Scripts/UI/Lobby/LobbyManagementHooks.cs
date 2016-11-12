using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class LobbyManagementHooks : MonoBehaviour {

    public void StartHost()
    {
        /*
        LobbyManager.s_Singleton.networkAddress = Network.player.ipAddress;
        print(LobbyManager.s_Singleton.networkAddress);
        LobbyManager.s_Singleton.StartHost();
        */
        NetworkLobbyManager.singleton.networkAddress = Network.player.ipAddress;
        print(NetworkLobbyManager.singleton.networkAddress);
        NetworkLobbyManager.singleton.StartHost();
    }

    public void StartLocal()
    {
        
    }

    public void StopHost()
    {
        LobbyManager.s_Singleton.StopClient();
    }

    public void AddPlayer()
    {
        LobbyManager.s_Singleton.AddLocalPlayer();
    }
}
