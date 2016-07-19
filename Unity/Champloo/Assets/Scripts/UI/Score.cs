using UnityEngine;
using System.Collections.Generic;

public class Score : MonoBehaviour {

    private List<int> scores;
    public List<int> Scores { get { return scores; } }

    private void Awake()
    {
        scores = new List<int>(FindObjectsOfType<Player>().Length);
        for(int i = 0; i < scores.Capacity; i++)
        {
            scores.Add(0);
        }
    }

    public void AddScore(int playerNumber)
    {
        scores[playerNumber - 1]++;
    }
    public void SubtractScore(int playerNumber)
    {
        if (scores[playerNumber - 1] > 0)
            scores[playerNumber - 1]--;
        //scores[playerNumber - 1]--;
    }
}
