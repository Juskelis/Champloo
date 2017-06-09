using Rewired;
using UnityEngine;

public class ControllerToggle : MonoBehaviour {
    public void SetMapCategory(string categoryName)
    {
        foreach (Rewired.Player player in ReInput.players.AllPlayers)
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, categoryName);
        }
    }
}
