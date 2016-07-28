using System;
using UnityEngine;
using System.Collections;

public class PlayerSettings : MonoBehaviour {

    /// <summary>
    /// Sets the indicated player's name so that it can be accessed across scenes/objects
    /// </summary>
    /// <param name="playerNumber">The one indexed player number, corresponding to controller number on local setup</param>
    /// <param name="name">The name you want the player to have</param>
    /// <returns>Returns whether or not the operation was successful. Only really applies for exceptions.</returns>
    public bool SetPlayerName(int playerNumber, string name)
    {
        try
        {
            print("setting player " + playerNumber + "'s name to '" + name + "'");
            PlayerPrefs.SetString("Name_" + playerNumber, name);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the indicated player's name
    /// </summary>
    /// <param name="playerNumber">The one indexed player number, corresponding to controller number on local setup</param>
    /// <returns>Returns the indicated player's name</returns>
    public string GetPlayerName(int playerNumber)
    {
        try
        {
            return PlayerPrefs.GetString("Name_" + playerNumber);
        }
        catch
        {
            return null;
        }
    }
}
