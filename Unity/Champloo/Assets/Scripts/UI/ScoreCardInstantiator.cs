using UnityEngine;
using System.Collections;

public class ScoreCardInstantiator : MonoBehaviour
{

    [SerializeField] private PlayerScoreCard scoreCardPrefab;

    private Score score;

    private bool lateStartDone = false;

    void Update()
    {
        if(!lateStartDone)
        {
            score = FindObjectOfType<Score>();

            for (int i = 0; i < score.Scores.Count; i++)
            {
                PlayerScoreCard currentPlayerScoreCard = Instantiate(scoreCardPrefab);
                currentPlayerScoreCard.playerNumber = i;
                currentPlayerScoreCard.transform.SetParent(transform, false);
            }
            Destroy(this);
        }
    }
}
