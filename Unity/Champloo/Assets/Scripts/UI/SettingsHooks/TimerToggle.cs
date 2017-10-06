using UnityEngine;

public class TimerToggle : MonoBehaviour {

    public void ToggleTimer(bool on)
    {
        PlayerSettings.TimerEnabled = on;
    } 
}
