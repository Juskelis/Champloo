using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField]
    private float spawnTime;

    private List<Player> players;

    private List<float> playerSpawnTimes;

    void Start()
    {
        // TODO : remove all present players from the list
        players = new List<Player>();

        playerSpawnTimes = new List<float>();
        foreach(Player p in FindObjectsOfType<Player>())
        {
            playerSpawnTimes.Add(0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    foreach(Player p in players)
        {
            playerSpawnTimes[p.PlayerNumber] = spawnTime;
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
        // TODO : Add master list of player prefabs
        //          for creating/deleting players
    }
}
