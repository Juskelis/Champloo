using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public InputField ipInput;
        public InputField matchNameInput;

        public RectTransform lobbyServerInfo;

        public void OnEnable()
        {
            lobbyManager.topPanel.ToggleVisibility(true);
            lobbyServerInfo.gameObject.SetActive(false);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            if (matchNameInput != null)
            {
                matchNameInput.onEndEdit.RemoveAllListeners();
                matchNameInput.onEndEdit.AddListener(onEndEditGameName);
            }
        }

        public void OnClickHost()
        {
            lobbyServerInfo.gameObject.SetActive(true);
            lobbyManager._isLocalMatch = false;
            lobbyManager.serverBindToIP = true;
            lobbyManager.StartHost();
            lobbyManager.playerSelectFlow.ShowMenu(lobbyManager.playerSelectStart);
        }

        public void OnClickPlayLocal()
        {
            lobbyServerInfo.gameObject.SetActive(false);
            lobbyManager._isLocalMatch = true;
            lobbyManager.serverBindToIP = false;
            lobbyManager.StartHost();
            lobbyManager.playerSelectFlow.ShowMenu(lobbyManager.playerSelectStart);
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);

            lobbyManager.playerSelectFlow.ShowMenu(lobbyManager.playerSelectStart);
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        public void OnClickCreateMatchmakingGame()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.matchMaker.CreateMatch(
                matchNameInput.text,
                (uint)lobbyManager.maxPlayers,
                true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);

            lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

    }
}
