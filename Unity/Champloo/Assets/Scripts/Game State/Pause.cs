using Rewired;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool GlobalPause = false;

    [SerializeField]
    private UnityEvent onPause;

    [SerializeField]
    private UnityEvent onResume;

    [SerializeField]
    private string pauseMenuCategory;

    [SerializeField]
    private UpdatePlayerNumber pauseName;

    private bool pressed = false;
    private bool paused = false;

    public void TogglePause()
    {
        if (!paused)
        {
            onPause.Invoke();
            if (GlobalPause)
            {
                Time.timeScale = 0;
            }
            paused = true;
        }
        else
        {
            onResume.Invoke();
            if (GlobalPause)
            {
                Time.timeScale = 1;
            }
            paused = false;
        }
    }

    public void LateUpdate()
    {
        int controllerID = -1;
        foreach (Rewired.Player player in ReInput.players.Players)
        {
            if (FindPlayerNumberFromController(player.id) < 0)
            {
                continue;
            }

            if (!pressed && (player.GetButtonDown("Pause") || player.GetButtonDown("Resume")))
            {
                pressed = true;
                controllerID = player.id;
                TogglePause();
            }
            else if (pressed && ((paused && player.GetButtonUp("Resume")) || (!paused && player.GetButtonUp("Pause"))))
            {
                pressed = false;
            }
        }
        if (paused && controllerID >= 0)
        {
            //FindObjectOfType<ControllerToggle>().DisableAllControllersExcept(controllerID);
            FindObjectOfType<ControllerToggle>().EnableAfter(pauseMenuCategory, controllerID);
            int pausingPlayerNumber = FindPlayerNumberFromController(controllerID);
            pauseName.UpdateText(pausingPlayerNumber);
        }
    }

    void OnDestroy()
    {
        if (GlobalPause)
        {
            Time.timeScale = 1;
        }
    }

    int FindPlayerNumberFromController(int controllerNumber)
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (player.InputPlayer.id == controllerNumber)
            {
                return player.PlayerNumber;
            }
        }
        return -1;
    }
}
