using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;

public class MapSelect : MonoBehaviour {
    public void SetMap(MultiplayerSelectable selectable, MultiplayerUIController controller)
    {
        MapOption option = selectable.GetComponent<MapOption>();
        if(option != null)
        {
            LobbyManager.s_Singleton.playScene = option.mapName;
        }
        else
        {
            Debug.LogError("Selectable does not have a MapOption");
        }
    }
}
