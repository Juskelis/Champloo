using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;

public class LobbyCodeHookTEST : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.I))
	    {
	        LobbyManager.s_Singleton.StartHost();
	    }
	    if (Input.GetKeyDown(KeyCode.H))
	    {
	        LobbyManager.s_Singleton.OnStartHost();
	    }
	    if (Input.GetKeyDown(KeyCode.Z))
	    {
	        LobbyManager.s_Singleton.StopClient();
	    }
	    if (Input.GetKeyDown(KeyCode.O))
	    {
	        LobbyManager.s_Singleton.StopHostClbk();
	    }
	    if (Input.GetKeyDown(KeyCode.J))
	    {
            LobbyManager.s_Singleton.AddLocalPlayer();
	    }
	    if (Input.GetKeyDown(KeyCode.K))
	    {
	        foreach (var player in FindObjectsOfType<LobbyPlayer>())
	        {
	            player.OnReadyClicked();
	        }
	    }
	    if (Input.GetKeyDown(KeyCode.C))
	    {
	        foreach (var player in FindObjectsOfType<LobbyPlayer>())
	        {
	            player.OnColorClicked();
	        }
	    }
	    if (Input.GetKeyDown(KeyCode.X))
	    {
	        LobbyManager.s_Singleton.RemovePlayer(FindObjectsOfType<LobbyPlayer>()[0]);
	    }
	}
}
