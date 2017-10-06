using System;
using UnityEngine;
using System.Collections;


public class PlayerSettings : MonoBehaviour
{
    public static bool TimerEnabled { get; set; }

    public Color Color;
    public string Name;
    public int LocaInputPlayerID;
    public string PrefabPath; //useful for Resources
    public Transform Prefab;  //used for AssetBundles

    public static PlayerSettings GetSettingsFor(int playerNumber)
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (player.PlayerNumber == playerNumber)
            {
                return player.GetComponent<PlayerSettings>();
            }
        }
        return null;
    }
}


//old version; uses playerprefs to store player info
/*
public class PlayerSettings : MonoBehaviour {

    /// <summary>
    /// Sets the indicated player's name so that it can be accessed across scenes/objects
    /// </summary>
    /// <param name="playerNumber">The one indexed player number, corresponding to controller number on local setup</param>
    /// <param name="name">The name you want the player to have</param>
    /// <returns>Returns whether or not the operation was successful.</returns>
    public bool SetPlayerName(int playerNumber, string name)
    {
        try
        {
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
            return "Yoshitsune";
        }
    }

    public bool SetPlayerPrefab(int playerNumber, string prefabName)
    {
        try
        {
            PlayerPrefs.SetString("Player_Prefab_" + playerNumber, prefabName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetPlayerPrefab(int playerNumber)
    {
        try
        {
            return PlayerPrefs.GetString("Player_Prefab_" + playerNumber);
        }
        catch
        {
            return "Prefabs/Player/Player";
        }
    }

    /// <summary>
    /// Set the total number of players
    /// </summary>
    /// <param name="num">The total number of players</param>
    /// <returns>Returns whether or not the operation was successful.</returns>
    public bool SetNumPlayers(int num)
    {
        try
        {
            PlayerPrefs.SetInt("NumPlayers", num);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the total number of players
    /// </summary>
    /// <returns>Returns the total number of players.</returns>
    public int GetNumPlayers()
    {
        try
        {
            return PlayerPrefs.GetInt("NumPlayers");
        }
        catch
        {
            return 0;
        }
    }


    public bool SetWeapon(int playerNumber, string weaponName)
    {
        try
        {
            PlayerPrefs.SetString("Weapon_" + playerNumber, weaponName);

            return true;
        }
        catch
        {
            return false;
        }
    }


    public string GetWeapon(int playerNumber)
    {
        try
        {
            return PlayerPrefs.GetString("Weapon_" + playerNumber);
        }
        catch
        {
            return "Prefabs/Weapon/Sword";
        }
    }

    public bool SetShield(int playerNumber, string shieldName)
    {
        try
        {
            PlayerPrefs.SetString("Shield_" + playerNumber, shieldName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GetShield(int playerNumber)
    {
        try
        {
            return PlayerPrefs.GetString("Shield_" + playerNumber);
        }
        catch
        {
            return "Prefabs/Shield/Shield";
        }
    }
}
*/
