using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;

public class MapSelect : MonoBehaviour {
    public void SetMap(VotingManager.Option electedMap)
    {
        MapOption option = electedMap.selectable.GetComponent<MapOption>();
        if(option != null)
        {
            print("Changing map to " + option.mapName);
            LobbyManager.s_Singleton.playScene = option.mapName;
        }
        else
        {
            Debug.LogError("Selectable does not have a MapOption");
        }
    }
}
