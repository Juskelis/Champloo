using System;
using UnityEngine;
using System.Collections.Generic;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine.Events;
using UnityEngine.UI;

public class MultiplayerUIManager : MonoBehaviour
{
    [SerializeField]
    private MultiplayerSelectable firstSelected;

    [SerializeField]
    private MultiplayerUIController controllerPrefab;

    [SerializeField]
    private string joinAction = "UIJoin";

    [SerializeField]
    private string leaveAction = "UILeave";

    [SerializeField]
    private UnityEvent OnPlayersLeave;

    [SerializeField]
    private UnityEvent OnAllSelected;
    private bool allSelected = false;

    private List<MultiplayerUIController> activeControllers;

    private bool playersJoined = false;

	// Use this for initialization
	void Start ()
	{
        activeControllers = new List<MultiplayerUIController>();

	}

    void OnEnable()
    {
        ReInput.ControllerPreDisconnectEvent += ReInputOnControllerPreDisconnectEvent;
        FindObjectOfType<RewiredStandaloneInputModule>().enabled = false;
        playersJoined = false;
    }

    void OnDisable()
    {
        ReInput.ControllerPreDisconnectEvent -= ReInputOnControllerPreDisconnectEvent;
        RewiredStandaloneInputModule module = FindObjectOfType<RewiredStandaloneInputModule>();
        if (module == null) return;
        module.enabled = true;
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
        if (activeControllers.Count == 0) return;
        int selected = 0;
        foreach(var controller in activeControllers)
        {
            if (controller.hasSelected) selected++;
        }
        if(!allSelected && selected == activeControllers.Count)
        {
            allSelected = true;
            OnAllSelected.Invoke();
        }
        else if(allSelected && selected != activeControllers.Count)
        {
            allSelected = false;
        }
	}

    private bool ContainsController(Rewired.Player p)
    {
        foreach (var controller in activeControllers)
        {
            if (controller.ControllerNumber == p.id) return true;
        }
        return false;
    }

    private void AddController(Rewired.Player p)
    {
        MultiplayerUIController controller = Instantiate(controllerPrefab);
        controller.ControllerNumber = p.id;
        controller.ChangeSelected(firstSelected);
        activeControllers.Add(controller);
        playersJoined = true;
    }

    private void RemoveController(Rewired.Player p)
    {
        for (int i = 0; i < activeControllers.Count; i++)
        {
            if (activeControllers[i].ControllerNumber == p.id)
            {
                Destroy(activeControllers[i].gameObject);
                activeControllers.RemoveAt(i);
            }
        }
        if (playersJoined && activeControllers.Count <= 0)
        {
            //all players have left!
            OnPlayersLeave.Invoke();
        }
    }
}
