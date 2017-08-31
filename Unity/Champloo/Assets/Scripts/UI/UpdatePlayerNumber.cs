﻿using UnityEngine;
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
        Color color = PlayerSettings.GetSettingsFor(winnerNumber).Color;
        ourText.text = prefix + " " + "<color=#" + Utility.ColorToHex(color) + ">" + name + "</color>";
    }

    public void UpdateText(int winnerNumber)
    {
        Text ourText = GetComponent<Text>();
        string name = PlayerSettings.GetSettingsFor(winnerNumber).Name;
        Color color = PlayerSettings.GetSettingsFor(winnerNumber).Color;
        ourText.text = prefix + " " + "<color=#" + Utility.ColorToHex(color) + ">" + name + "</color>";
    }
}
