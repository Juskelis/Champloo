using System;
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
    public float SpawnTime { get { return spawnTime; } private set { spawnTime = value; } }

    [SerializeField]
    private List<Player> players;

    [SerializeField]
    [Tooltip("Spaces out colliders when finding spawn points")]
    private float sizeMultiplier = 1f;

    private List<float> playerSpawnTimes;

    void Awake()
    {
        /*
        players = new List<Player>();
        playerSpawnTimes = new List<float>();

        foreach (var player in FindObjectsOfType<Player>())
        {
            players.Add(player);
            playerSpawnTimes.Add(spawnTime);
        }

        PlayerSettings settings = FindObjectOfType<PlayerSettings>();
        int numPlayers = settings.GetNumPlayers();
        for (int i = numPlayers; i < 2; i++)
        {
            settings.SetPlayerName(i + 1, "Yoshitsune");
            settings.SetPlayerPrefab(i + 1, "Prefabs/Player/Player");
            settings.SetShield(i + 1, "Prefabs/Shield/Shield");
            settings.SetWeapon(i + 1, "Prefabs/Weapon/Sword");
            numPlayers++;
        }

        print("Num Players: " + numPlayers);
        for (int i = 0; i < numPlayers; i++)
        {
            string prefabName = settings.GetPlayerPrefab(i + 1);
            string weaponName = settings.GetWeapon(i + 1);
            string shieldName = settings.GetShield(i + 1);
            
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
	void Update ()
	{
        /*
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
        */
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

    public Vector3 FindValidSpawn(Player p, int maxAttempts = 30)
    {
        BoxCollider2D col = p.GetComponent<BoxCollider2D>();

        SpawnZone zone;

        Vector2 test = Vector2.zero;
        bool valid = false;
        int attempts = 0;

        while(attempts < maxAttempts && !valid)
        {
            zone = bestZone();
            test.x = UnityEngine.Random.Range(zone.topLeft.x, zone.bottomRight.x);
            test.y = UnityEngine.Random.Range(zone.bottomRight.y, zone.topLeft.y);
            
            Collider2D[] cols = Physics2D.OverlapBoxAll(test, col.size * sizeMultiplier, 0f);
            if (cols == null || cols.Length <= 0)
            {
                valid = true;
            }

            attempts++;
        }

        if(attempts > maxAttempts)
        {
            return transform.position;
        }
        return test;
    }

    private SpawnZone bestZone()
    {
        SpawnZone farthestSpawn = spawnZones[0];
        float maxDist = distanceFromPlayers(farthestSpawn);
        foreach (SpawnZone spawnZone in spawnZones)
        {
            float distance = distanceFromPlayers(spawnZone);
            if (distance > maxDist)
            {
                farthestSpawn = spawnZone;
                maxDist = distance;
            }
        }
        return farthestSpawn;
    }

    /// <summary>
    /// Gets the distance to the nearest player
    /// </summary>
    private float distanceFromPlayers(SpawnZone zone)
    {
        Vector3 center = (zone.bottomRight + zone.topLeft)/2;
        float minDistance = float.MaxValue;
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.Dead) continue;

            float sqrDistance = Vector3.SqrMagnitude(player.transform.position - center);
            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
            }
        }
        return minDistance;
    }
}
