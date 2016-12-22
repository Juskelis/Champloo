using System;
using UnityEngine;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class UIManagerEvent : UnityEvent<Rewired.Player> { }

public class MultiplayerUIManager : MonoBehaviour
{
    [SerializeField]
    private bool autoAddPlayers = false;

    [SerializeField]
    private MultiplayerSelectable firstSelected;

    [SerializeField]
    private MultiplayerUIController controllerPrefab;

    [SerializeField]
    private string joinAction = "UIJoin";

    [SerializeField]
    private string leaveAction = "UILeave";

    [SerializeField]
    private UIManagerEvent OnPlayerJoin;

    [SerializeField]
    private UIManagerEvent OnPlayerLeave;

    [SerializeField]
    private UnityEvent OnAllPlayersLeave;
    
    [SerializeField]
    private UnityEvent OnAllSelected;
    private bool allSelected = false;

    [SerializeField]
    private UnityEvent OnEnabled;

    private List<MultiplayerUIController> activeControllers;

    private static Dictionary<Rewired.Player, bool> hasJoinedDictionary;

    private bool playersJoined = false;

	// Use this for initialization
	void Awake ()
	{
        activeControllers = new List<MultiplayerUIController>();
        if(hasJoinedDictionary == null)
            hasJoinedDictionary = new Dictionary<Rewired.Player, bool>();
	}

    void OnEnable()
    {
        ReInput.ControllerPreDisconnectEvent += ReInputOnControllerPreDisconnectEvent;
        FindObjectOfType<RewiredStandaloneInputModule>().enabled = false;
        playersJoined = false;
        allSelected = false;
        if (autoAddPlayers)
        {
            List<Rewired.Player> toAdd = new List<Rewired.Player>();
            foreach (var pair in hasJoinedDictionary)
            {
                if(pair.Value) toAdd.Add(pair.Key);
            }
            foreach (var player in toAdd)
            {
                AddController(player, true);
            }
        }
        OnEnabled.Invoke();
    }

    void OnDisable()
    {
        ReInput.ControllerPreDisconnectEvent -= ReInputOnControllerPreDisconnectEvent;
        RewiredStandaloneInputModule module = FindObjectOfType<RewiredStandaloneInputModule>();
        if (module == null) return;
        module.enabled = true;

        //clear all active controllers
        foreach (var controller in activeControllers)
        {
            Destroy(controller.gameObject);
        }
        activeControllers.Clear();
    }

    private void ReInputOnControllerPreDisconnectEvent(ControllerStatusChangedEventArgs controllerStatusChangedEventArgs)
    {
        foreach (var player in ReInput.players.Players)
        {
            if (player.controllers.ContainsController(
                controllerStatusChangedEventArgs.controllerType,
                controllerStatusChangedEventArgs.controllerId))
            {
                RemoveController(player);
            }
        }
    }

    // Update is called once per frame
	void Update () {
	    foreach (var player in ReInput.players.Players)
	    {
	        if (player.GetButtonDown(joinAction) && !ContainsController(player))
	        {
	            //player has joined
                AddController(player);
	        }
	        if (player.GetButtonDown(leaveAction) && ContainsController(player))
	        {
	            //player has left
                RemoveController(player);
	        }
	    }

        //check all the controllers
        if (LobbyManager.s_Singleton.lobbySlots.Length == 0)
        {
            allSelected = false;
            return;
        }

	    bool noneSelected = true;
	    bool allHaveSelected = true;
	    LobbyPlayer lobbyPlayer;
	    foreach (var netPlayer in LobbyManager.s_Singleton.lobbySlots)
	    {
            if (!netPlayer) continue;
	        lobbyPlayer = (LobbyPlayer) netPlayer;
	        if (lobbyPlayer.activated)
	        {
	            noneSelected = false;
	        }
	        else
	        {
	            allHaveSelected = false;
	        }
	    }
	    if (noneSelected && allSelected)
	    {
	        allSelected = false;
	    }
	    else if (allHaveSelected && !allSelected)
	    {
	        allSelected = true;
            OnAllSelected.Invoke();
	    }
	}

    void LateUpdate()
    {
        //update lobbyplayers with correct info
        foreach (var controller in activeControllers)
        {
            foreach (var player in GetLocalPlayers(controller.ControllerNumber))
            {
                player.selectedSelectable = controller.CurrentlySelected.ToString();
            }
        }
    }

    private bool ContainsController(Rewired.Player p)
    {
        return hasJoinedDictionary.ContainsKey(p) && hasJoinedDictionary[p];
    }

    private void AddController(Rewired.Player p, bool startupAdd = false)
    {
        MultiplayerUIController controller = Instantiate(controllerPrefab);
        controller.ControllerNumber = p.id;
        controller.ChangeSelected(firstSelected);
        activeControllers.Add(controller);
        playersJoined = true;
        hasJoinedDictionary[p] = true;
        if (!startupAdd)
        {
            OnPlayerJoin.Invoke(p);
            AddLocalPlayer(p.id);
        }
    }

    private void AddLocalPlayer(int controllerNumber)
    {
        LobbyManager.s_Singleton.AddLocalPlayer();
        LobbyPlayer lp;
        foreach (var networkLobbyPlayer in LobbyManager.s_Singleton.lobbySlots)
        {
            lp = networkLobbyPlayer as LobbyPlayer;
            if (lp != null && lp.isLocalPlayer && lp.playerControllerNumber < 0)
            {
                lp.playerControllerNumber = controllerNumber;
            }
        }
    }

    private void RemoveLocalPlayer(int controllerNumber)
    {
        List<LobbyPlayer> toRemove = GetLocalPlayers(controllerNumber);
        foreach (LobbyPlayer t in toRemove)
        {
            LobbyManager.s_Singleton.RemovePlayer(t);
        }
    }

    private List<LobbyPlayer> GetLocalPlayers(int controllerNumber)
    {
        List<LobbyPlayer> ret = new List<LobbyPlayer>();
        LobbyPlayer toAdd;
        foreach (var nlp in LobbyManager.s_Singleton.lobbySlots)
        {
            toAdd = nlp as LobbyPlayer;
            if (toAdd != null && toAdd.isLocalPlayer && toAdd.playerControllerNumber == controllerNumber)
            {
                ret.Add(toAdd);
            }
        }
        return ret;
    }

    private void RemoveController(Rewired.Player p)
    {
        for (int i = 0; i < activeControllers.Count; i++)
        {
            if (activeControllers[i].ControllerNumber == p.id)
            {
                RemoveLocalPlayer(p.id);
                Destroy(activeControllers[i].gameObject);
                activeControllers.RemoveAt(i);
            }
        }
        hasJoinedDictionary[p] = false;
        OnPlayerLeave.Invoke(p);

        if (playersJoined && activeControllers.Count <= 0)
        {
            //all players have left!
            OnAllPlayersLeave.Invoke();
        }
    }
}
