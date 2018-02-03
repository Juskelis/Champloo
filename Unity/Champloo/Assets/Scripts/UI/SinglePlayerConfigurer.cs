using Prototype.NetworkLobby;
using UnityEngine;

public class SinglePlayerConfigurer : MonoBehaviour
{

    private int defaultMinPlayers;
    private int defaultMaxPlayers;

    private LobbyManager manager;

    void Start()
    {
        manager = LobbyManager.s_Singleton ?? FindObjectOfType<LobbyManager>();
        defaultMinPlayers = manager.minPlayers;
        defaultMaxPlayers = manager.maxPlayers;
    }

    public void SetSinglePlayer(bool singlePlayer)
    {
        manager.minPlayers = singlePlayer ? 1 : defaultMinPlayers;
        manager.maxPlayers = singlePlayer ? 1 : defaultMaxPlayers;
        manager.maxPlayersPerConnection = singlePlayer ? 1 : defaultMaxPlayers;
        PlayerSettings.TimerEnabled = !singlePlayer && PlayerSettings.TimerEnabled;
    }
}
