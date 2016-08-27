using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.Events;

public class PlayerSelectConfirmation : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onReady;

    [SerializeField]
    private UnityEvent onReturn;

    private int confirmedPlayers = 0;

    private int possibleConfirmed = 0;

    private void Update()
    {
        int totalPlayers = 0;
        possibleConfirmed = 0;
        confirmedPlayers = 0;
        foreach (var child in GetComponentsInChildren<PlayerSelect>())
        {
            totalPlayers++;
            if (child.Working) possibleConfirmed++;
            if (child.Finished) confirmedPlayers++;
        }

        if (possibleConfirmed > 1 && confirmedPlayers == possibleConfirmed)
        {
            onReady.Invoke();
            FindObjectOfType<PlayerSettings>().SetNumPlayers(confirmedPlayers);
        }

        if (totalPlayers > 0 && possibleConfirmed < 1 && GamePad.GetButtonDown(GamePad.Button.B, GamePad.Index.Any))
        {
            //return to previous menu
            onReturn.Invoke();
        }
    }
}
