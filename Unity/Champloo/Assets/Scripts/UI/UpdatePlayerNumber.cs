using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerNumber : MonoBehaviour
{
    [SerializeField]
    string prefix = "";

    public void UpdateText()
    {
        Text ourText = GetComponent<Text>();
        int winnerNumber = Score.instance.FindWinner();
        string name = PlayerSettings.GetSettingsFor(winnerNumber).Name;
        ourText.text = prefix + " " + name + "(" + winnerNumber + ")";
    }
}
