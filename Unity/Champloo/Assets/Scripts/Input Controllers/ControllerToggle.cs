using Rewired;
using UnityEngine;

public class ControllerToggle : MonoBehaviour {

    [SerializeField]
    private float enableDelay = 1f;
    
    private string catergoryToEnable;
    private int controllerToEnable;

    public void SetMapCategory(string categoryName)
    {
        foreach (Rewired.Player player in ReInput.players.AllPlayers)
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, categoryName);
        }
    }

    public void DisableAllControllersExcept(int playerControllerNumber)
    {
        foreach (Rewired.Player player in ReInput.players.AllPlayers)
        {
            if (player.id != playerControllerNumber)
            {
                player.controllers.maps.SetAllMapsEnabled(false);
            }
        }
    }

    public void DisableAllControllers()
    {
        foreach(Rewired.Player p in ReInput.players.AllPlayers)
        {
            p.controllers.maps.SetAllMapsEnabled(false);
        }
    }

    public void EnableAllAfter(string category)
    {
        CancelInvoke();
        DisableAllControllers();
        catergoryToEnable = category;
        Invoke("TimedEnableAll", enableDelay);
    }

    public void EnableAfter(string catergory, int controllerNumber)
    {
        CancelInvoke();
        DisableAllControllers();
        catergoryToEnable = catergory;
        controllerToEnable = controllerNumber;
        Invoke("TimedEnable", enableDelay);
    }

    private void TimedEnableAll()
    {
        SetMapCategory(catergoryToEnable);
    }

    private void TimedEnable()
    {
        SetMapCategory(catergoryToEnable);
        DisableAllControllersExcept(controllerToEnable);
    }
}
