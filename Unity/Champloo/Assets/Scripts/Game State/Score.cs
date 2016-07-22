using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class Score : MonoBehaviour
{

    [SerializeField]
    private int winScore = 7;

    public int WinScore
    {
        get { return winScore; }
    }

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
        if (scores[playerNumber - 1] >= winScore)
        {
            GetComponent<Match>().End();
        }
    }
    public void SubtractScore(int playerNumber)
    {
        if (scores[playerNumber - 1] > 0)
            scores[playerNumber - 1]--;
    }

    public bool IsTied()
    {
        int max = scores.Max();
        int shareMax = 0;
        foreach (int score in scores)
        {
            if (score == max) shareMax++;
        }

        return shareMax > 1;
    }

    //returns winner playerNumber
    public int FindWinner()
    {
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] >= winScore) return i;
        }

        return -1;
    }
}
