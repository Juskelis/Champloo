using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;

public class PlayerCardInstantiator : MonoBehaviour
{

    [SerializeField] private PlayerCard cardPrefab;

    [Header("Dictionary<Player, PlayerCard>")]
    [SerializeField] private List<Player> playerPrefabs;
    [SerializeField] private List<PlayerCard> cardPrefabs;

    private bool lateStartDone = false;

    void Update()
    {
        if(!lateStartDone)
        {
            foreach (var player in LobbyManager.FindObjectsOfType<Player>())
            {
                int index = playerPrefabs.FindIndex(player1 => 
                    player1.name.Contains(player.name) || player.name.Contains(player1.name));
                if (index < 0 || index > cardPrefabs.Count)
                {
                    Debug.LogError("Could not find player with prefab name " + player.name);
                    continue;
                }
                PlayerCard toSpawn = cardPrefabs[index];

                PlayerCard currentPlayerCard = Instantiate(toSpawn);
                currentPlayerCard.PlayerNumber = player.PlayerNumber;
                currentPlayerCard.transform.SetParent(transform, false);
            }
            Destroy(this);
        }
        lateStartDone = true;
    }
}
