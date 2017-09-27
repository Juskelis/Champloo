using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class VotingTimer : MonoBehaviour
{
    public Text timer;

    public VotingManager manager;

    void Update()
    {
        timer.enabled = manager.hasVotingTimeout && manager.VotingTimeLeft > 0;
        timer.text = Mathf.CeilToInt(manager.VotingTimeLeft).ToString(CultureInfo.InvariantCulture);
    }
}
