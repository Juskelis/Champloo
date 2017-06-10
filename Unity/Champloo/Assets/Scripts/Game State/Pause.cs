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

    private bool pressed = false;
    private bool paused = false;

    public void TogglePause()
    {
        if (!paused)
        {
            onPause.Invoke();
            paused = true;
        }
        else
        {
            onResume.Invoke();
            paused = false;
        }
    }

    public void LateUpdate()
    {
        foreach (Rewired.Player player in ReInput.players.Players)
        {
            if (!pressed && (player.GetButtonDown("Pause") || player.GetButtonDown("Resume")))
            {
                pressed = true;
                TogglePause();
            }
            else if (pressed && ((paused && player.GetButtonUp("Resume")) || (!paused && player.GetButtonUp("Pause"))))
            {
                pressed = false;
            }
        }
    }
}
