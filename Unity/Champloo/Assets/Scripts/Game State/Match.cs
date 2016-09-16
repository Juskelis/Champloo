using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Events;

public class Match : MonoBehaviour
{

    [SerializeField]
    private UnityEvent onStart;

    [SerializeField]
    private UnityEvent onEnd;

    [SerializeField]
    private UnityEvent onWin;

    [SerializeField]
    private UnityEvent onTie;

    [SerializeField]
    private UnityEvent onLeave;
    [SerializeField]
    private float secondsToLeave;

    public void Start()
    {
        /*
        onStart.Invoke();
        SetPlayerInput(true);
        */
        Invoke("MatchStart", 0.001f);
    }

    public void MatchStart()
    {
        onStart.Invoke();
        SetPlayerInput(true);
    }

    public void SetPlayerInput(bool active)
    {
        //InputController.SetInputs(active);
        /*
        foreach (var ic in FindObjectsOfType<InputController>())
        {
            ic.inputPlayer.controllers.maps.SetMapsEnabled(active, "In Game");
            ic.inputPlayer.controllers.maps.SetMapsEnabled(!active, "Menu");
        }
        */

        int i = 0;
        Rewired.Player p = Utility.GetNetworkPlayer(i);
        while (p != null)
        {
            p.controllers.maps.SetMapsEnabled(active, "In Game");
            p.controllers.maps.SetMapsEnabled(!active, "Menu");
            p = Utility.GetNetworkPlayer(++i);
        }
    }

    public void End()
    {
        onEnd.Invoke();

        if (GetComponent<Score>().IsTied())
        {
            onTie.Invoke();
        }
        else
        {
            onWin.Invoke();
        }

        Invoke("Leave", secondsToLeave);
    }

    public void Leave()
    {
        onLeave.Invoke();
    }
}
