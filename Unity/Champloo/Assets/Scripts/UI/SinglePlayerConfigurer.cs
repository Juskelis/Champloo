using Prototype.NetworkLobby;
using UnityEngine;

public class SinglePlayerConfigurer : MonoBehaviour
{

    private int defaultMinPlayers;
    private int defaultMaxPlayers;

    private LobbyManager manager;

    private MultiplayerUIManager[] uiManagers;

    void Start()
    {
        manager = LobbyManager.s_Singleton ?? FindObjectOfType<LobbyManager>();
        uiManagers = FindObjectsOfType<MultiplayerUIManager>();
        foreach(MultiplayerUIManager manager in uiManagers)
        {
            manager.MinPlayersToAdvance = 2;
        }
        defaultMinPlayers = manager.minPlayers;
        defaultMaxPlayers = manager.maxPlayers;
    }

    public void SetSinglePlayer(bool singlePlayer)
    {
        manager.minPlayers = singlePlayer ? 1 : defaultMinPlayers;
        manager.maxPlayers = singlePlayer ? 1 : defaultMaxPlayers;
        manager.maxPlayersPerConnection = singlePlayer ? 1 : defaultMaxPlayers;
        PlayerSettings.TimerEnabled = !singlePlayer && PlayerSettings.TimerEnabled;

        foreach (MultiplayerUIManager manager in uiManagers)
        {
            if(singlePlayer)
            {
                manager.MinPlayersToAdvance = 1;
            } else
            {
                manager.MinPlayersToAdvance = 2;
            }
        }
    }
}
