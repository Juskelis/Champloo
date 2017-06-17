using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;

public class ScoreCardInstantiator : MonoBehaviour
{

    [SerializeField] private PlayerScoreCard scoreCardPrefab;

    private bool lateStartDone = false;

    void Update()
    {
        if(!lateStartDone)
        {
            foreach (var player in LobbyManager.FindObjectsOfType<Player>())
            {
                PlayerScoreCard currentPlayerScoreCard = Instantiate(scoreCardPrefab);
                currentPlayerScoreCard.playerNumber = player.PlayerNumber;
                currentPlayerScoreCard.transform.SetParent(transform, false);
            }
            Destroy(this);
        }
    }
}
