using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerNumber : MonoBehaviour
{

    public void UpdateText()
    {
        Text ourText = GetComponent<Text>();
        /*
        string replacement = FindObjectOfType<PlayerSettings>().GetPlayerName(FindObjectOfType<Score>().FindWinner());

        Regex rgx = new Regex("\\d+");

        string text = rgx.Replace(ourText.text, replacement);
        */

        Regex find = new Regex("\\[\\d+\\]");

        string match = find.Match(ourText.text).Value;
        print("Match is: '" + match + "'");
        int conversion = int.Parse(match.Substring(1, match.Length - 2));

        string text = "";

        if(conversion <= 0)
        {
            //text = FindObjectOfType<PlayerSettings>().GetPlayerName(FindObjectOfType<Score>().FindWinner());
            text = PlayerSettings.GetSettingsFor(Score.instance.FindWinner()).Name;
        }
        else
        {
            //text = FindObjectOfType<PlayerSettings>().GetPlayerName(conversion);
            text = PlayerSettings.GetSettingsFor(conversion).Name;
        }

        ourText.text = find.Replace(ourText.text, text);
        
        Destroy(this);
    }
}
