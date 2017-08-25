using Rewired;
using UnityEngine;
using UnityEngine.Events;
using Player = Rewired.Player;

public class Pause : MonoBehaviour
{
    public bool GlobalPause = false;

    [SerializeField]
    private UnityEvent onPause;

    [SerializeField]
    private UnityEvent onResume;

    [SerializeField]
    private string pauseMenuCategory;

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
        }
    }

    void OnDestroy()
    {
        if (GlobalPause)
        {
            Time.timeScale = 1;
        }
    }
}
