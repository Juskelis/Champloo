using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerSettings playerSettings = gamePlayer.GetComponent<PlayerSettings>();

        playerSettings.Name = lobby.playerName;
        playerSettings.Color = lobby.playerColor;
    }
}
