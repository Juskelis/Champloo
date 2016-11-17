using System;
using UnityEngine;
using System.Collections.Generic;
using Rewired;
using Rewired.Integration.UnityUI;
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

    private List<MultiplayerUIController> activeControllers;

	// Use this for initialization
	void Start ()
	{
        activeControllers = new List<MultiplayerUIController>();

	}

    void OnEnable()
    {
        print("OnEnable");
        ReInput.ControllerPreDisconnectEvent += ReInputOnControllerPreDisconnectEvent;
        FindObjectOfType<RewiredStandaloneInputModule>().enabled = false;
    }

    void OnDisable()
    {
        print("OnDisable");
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
        print("add player");
        MultiplayerUIController controller = Instantiate(controllerPrefab);
        controller.ControllerNumber = p.id;
        controller.ChangeSelected(firstSelected);
        activeControllers.Add(controller);
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
    }
}
