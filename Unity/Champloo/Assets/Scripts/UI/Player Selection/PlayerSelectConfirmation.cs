using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerSelectConfirmation : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onReady;

    private int confirmedPlayers = 0;

    private int possibleConfirmed = 0;

    private void Update()
    {
        possibleConfirmed = 0;
        confirmedPlayers = 0;
        foreach (var child in GetComponentsInChildren<PlayerSelect>())
        {
            if (child.Working) possibleConfirmed++;
            if (child.Finished) confirmedPlayers++;
        }

        if (possibleConfirmed > 1 && confirmedPlayers == possibleConfirmed)
        {
            onReady.Invoke();
            FindObjectOfType<PlayerSettings>().SetNumPlayers(confirmedPlayers);
        }
    }
}
