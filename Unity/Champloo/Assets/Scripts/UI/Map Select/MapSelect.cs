using System;
using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour {
    public void SetMap(VotingManager.Option electedMap)
    {
        MapOption option = electedMap.selectable.GetComponent<MapOption>();
        if(option != null)
        {
            LobbyManager.s_Singleton.playScene = option.mapName;
        }
        else
        {
            Debug.LogError("Selectable does not have a MapOption");
        }
    }

    public void SetMap(String mapName)
    {
        LobbyManager.s_Singleton.playScene = mapName;
    }
}
