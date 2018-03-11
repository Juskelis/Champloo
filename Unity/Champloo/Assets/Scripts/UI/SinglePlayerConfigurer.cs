using Prototype.NetworkLobby;
using UnityEngine;

/// <summary>
/// You may be wondering "why are the default values static? why are only setting them if theyre less than zero???"
/// It turns out that Awake is called every time the script is LOADED, which happens every time the scene changes.
/// The script actually resets itself entirely for all instance variables. This is *bad* for something that is
/// persisting across scenes, since itll get mixed up in between scenes.
/// </summary>
public class SinglePlayerConfigurer : MonoBehaviour
{
    private static int defaultMinPlayers = -1;
    private static int defaultMaxPlayers = -1;

    private LobbyManager manager;

    private MultiplayerUIManager[] uiManagers;

    void Awake()
    {
        manager = LobbyManager.s_Singleton ?? FindObjectOfType<LobbyManager>();
        uiManagers = FindObjectsOfType<MultiplayerUIManager>();

        //awake gets called multiple times turns out,
        // so we need a way to keep track of ACTUAL awake
        if (defaultMaxPlayers < 0 || defaultMinPlayers < 0)
        {
            defaultMinPlayers = manager.minPlayers;
            defaultMaxPlayers = manager.maxPlayers;
            foreach (MultiplayerUIManager manager in uiManagers)
            {
                manager.MinPlayersToAdvance = defaultMinPlayers;
            }
        }
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
