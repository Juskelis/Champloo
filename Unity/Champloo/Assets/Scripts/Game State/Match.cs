using UnityEngine;
using System.Collections;
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
        onStart.Invoke();
    }

    public void SetPlayerInput(bool active)
    {
        InputController.SetInputs(active);
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
