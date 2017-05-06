using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Prototype.NetworkLobby;
using UnityEngine.Events;

public class PlayerSelectManager : MonoBehaviour
{
    public List<Player> playerPrefabs;

    public UnityEvent OnAllConfirmed;

    private static PlayerSelectManager _instance;

    public static PlayerSelectManager Instance
    {
        get { return _instance; }
    }

    public void Awake()
    {
        _instance = this;
    }

    public void AllSelectedCallback()
    {
        if (!gameObject.activeInHierarchy) return;

        OnAllConfirmed.Invoke();
    }
    
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
                if(player != null && player.isLocalPlayer && player.playerControllerNumber == controller.ControllerNumber)
                {
                    player.OnPrefabChanged(selectedOption.playerPrefabIndex);
                }
            }
        }
        else
        {
            Debug.Log("Selectable does not have a PlayerOption attached to it!");
        }
    }
}
