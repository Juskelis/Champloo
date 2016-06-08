using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField]
    private float spawnTime;

    [SerializeField]
    private List<Player> players;

    private List<float> playerSpawnTimes;

    void Start()
    {
        Player[] foundPlayers = FindObjectsOfType<Player>();
        playerSpawnTimes = new List<float>();
        players = new List<Player>();
        foreach(Player p in foundPlayers)
        {
            players.Add(p);
            playerSpawnTimes.Add(spawnTime);
        }
        players.Sort(delegate(Player x, Player y)
        {
            if (x == null && y == null) return 0;
            else if (x == null) return -1;
            else if (y == null) return 1;
            else return x.PlayerNumber.CompareTo(y.PlayerNumber);
        });
    }
	
	// Update is called once per frame
	void Update () {
	    foreach(Player p in FindObjectsOfType<Player>())
        {
            playerSpawnTimes[p.PlayerNumber-1] = spawnTime;
        }

        for(int i = 0; i < playerSpawnTimes.Count; i++)
        {
            playerSpawnTimes[i] -= Time.deltaTime;
            if(playerSpawnTimes[i] <= 0)
            {
                SpawnPlayer(i);
                playerSpawnTimes[i] = spawnTime;
            }
        }
	}

    void SpawnPlayer(int playerNumber)
    {
        if (playerNumber < 0 || playerNumber >= players.Count)
        {
            throw new ArgumentException("Tried to spawn invalid player number: " + playerNumber);
        }

        Player p = players[playerNumber];
        p.transform.position = transform.position;
        p.gameObject.SetActive(true);
    }
}
