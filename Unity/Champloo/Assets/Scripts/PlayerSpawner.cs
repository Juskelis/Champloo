﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour {
    [Serializable]
    struct SpawnZone
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;
    }

    [SerializeField]
    private SpawnZone[] spawnZones;

    [SerializeField]
    private float spawnTime;

    [SerializeField]
    private List<Player> players;

    private List<float> playerSpawnTimes;

    void Start()
    {
        players = new List<Player>();
        playerSpawnTimes = new List<float>();

        PlayerSettings settings = FindObjectOfType<PlayerSettings>();
        int numPlayers = settings.GetNumPlayers();
        print("Num Players: " + numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            string prefabName = "Prefabs/Player/Player";//settings.GetPlayerPrefab(i + 1);
            string weaponName = "Prefabs/Weapon/Sword";//settings.GetWeapon(i + 1);
            string shieldName = "Prefabs/Shield/Shield";//settings.GetShield(i + 1);

            Transform player = ((GameObject)Instantiate(Resources.Load(prefabName))).transform;
            Transform weapon = ((GameObject)Instantiate(Resources.Load(weaponName))).transform;
            Transform shield = ((GameObject)Instantiate(Resources.Load(shieldName))).transform;

            Player p = player.GetComponent<Player>();
            p.PlayerNumber = i + 1;

            weapon.SetParent(player);
            shield.SetParent(player);

            player.position = FindValidSpawn(p);

            players.Add(player.GetComponent<Player>());
            playerSpawnTimes.Add(spawnTime);
        }

        /*
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
        */
    }

    void OnDrawGizmos()
    {
        foreach(SpawnZone zone in spawnZones)
        {
            Gizmos.DrawWireCube((zone.topLeft + zone.bottomRight) / 2, (zone.topLeft - zone.bottomRight));
            Gizmos.DrawWireSphere(zone.topLeft, 0.1f);
            Gizmos.DrawWireSphere(zone.bottomRight, 0.1f);
        }
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
        p.transform.position = FindValidSpawn(p);
        p.gameObject.SetActive(true);
        p.Start();
    }

    Vector3 FindValidSpawn(Player p, int maxAttempts = 30)
    {
        BoxCollider2D col = p.GetComponent<BoxCollider2D>();

        Vector2 min = new Vector2(-9, -4);
        Vector2 max = new Vector2(9, 4);

        SpawnZone zone;

        Vector2 test = Vector2.zero;
        bool valid = false;
        int attempts = 0;

        while(attempts < maxAttempts && !valid)
        {
            zone = spawnZones[UnityEngine.Random.Range(0, spawnZones.Length)];
            test.x = UnityEngine.Random.Range(zone.topLeft.x, zone.bottomRight.x);
            test.y = UnityEngine.Random.Range(zone.bottomRight.y, zone.topLeft.y);

            if(!CollisionChecking.Place.Meeting(col, test, 0f))
            {
                valid = true;
            }
        }

        if(attempts > maxAttempts)
        {
            return transform.position;
        }
        return test;
    }
}
