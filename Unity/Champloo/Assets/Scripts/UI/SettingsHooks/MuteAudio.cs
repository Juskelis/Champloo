using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteAudio : MonoBehaviour {

    public void ToggleAudio(bool on)
    {
        AudioListener.volume = on ? 1 : 0;
    }
}
