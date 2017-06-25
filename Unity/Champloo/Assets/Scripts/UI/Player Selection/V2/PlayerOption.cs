using UnityEngine;
using System.Collections.Generic;

public class PlayerOption : MonoBehaviour
{
    public Player option;

    public int playerPrefabIndex
    {
        get
        {
            return PlayerSelectManager.Instance.playerPrefabs.FindIndex(
                player => player == option);
        }
    }

    public Sprite playerPortrait;
}
