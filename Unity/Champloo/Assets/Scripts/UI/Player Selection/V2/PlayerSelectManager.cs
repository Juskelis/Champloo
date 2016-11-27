using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Prototype.NetworkLobby;

public class PlayerSelectManager : MonoBehaviour {
    
    public void PlayerSelected(MultiplayerSelectable selectable, MultiplayerUIController controller)
    {
        PlayerOption selectedOption = selectable.GetComponent<PlayerOption>();

        if(selectedOption != null)
        {
            //find the appropriate LobbyPlayer and update information
            LobbyPlayer player;
            foreach(var networkPlayer in LobbyManager.s_Singleton.lobbySlots)
            {
                player = networkPlayer as LobbyPlayer;
                if(player != null && player.isLocalPlayer && player.playerControllerId == controller.ControllerNumber)
                {
                    print("updating lobby player info");
                    player.playerName = selectedOption.playerPrefabName;
                }
            }
        }
        else
        {
            Debug.Log("Selectable does not have a PlayerOption attached to it!");
        }
    }
}
