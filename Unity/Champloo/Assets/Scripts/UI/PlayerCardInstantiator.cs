using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;

public class PlayerCardInstantiator : MonoBehaviour
{

    [SerializeField] private PlayerCard cardPrefab;

    private bool lateStartDone = false;

    void Update()
    {
        if(!lateStartDone)
        {
            foreach (var player in LobbyManager.FindObjectsOfType<Player>())
            {
                PlayerCard currentPlayerCard = Instantiate(cardPrefab);
                currentPlayerCard.PlayerNumber = player.PlayerNumber;
                currentPlayerCard.transform.SetParent(transform, false);
            }
            Destroy(this);
        }
    }
}
