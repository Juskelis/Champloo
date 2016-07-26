using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerNumber : MonoBehaviour
{

    public void UpdateText()
    {
        Text ourText = GetComponent<Text>();
        
        string replacement = FindObjectOfType<PlayerSettings>().GetPlayerName(FindObjectOfType<Score>().FindWinner());

        Regex rgx = new Regex("\\d+");

        string text = rgx.Replace(ourText.text, replacement);

        ourText.text = text;

        Destroy(this);
    }
}
