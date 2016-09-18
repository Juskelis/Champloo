﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Score : NetworkBehaviour
{

    [SerializeField]
    private int winScore = 7;

    public int WinScore
    {
        get { return winScore; }
    }

    private List<int> scores;
    public List<int> Scores { get { return scores; } }

    bool lateStartDone = false;

    private void Update()
    {
        if (!isServer) return;

        if(!lateStartDone)
        {
            scores = new List<int>(FindObjectsOfType<Player>().Length);
            for (int i = 0; i < scores.Capacity; i++)
            {
                scores.Add(0);
            }
            lateStartDone = true;
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

    /// <summary>
    /// Find the index of the player in the lead
    /// </summary>
    /// <returns>The controller number (1 indexed) of the winning player</returns>
    public int FindWinner()
    {
        int max = scores.Max();
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] >= max) return i + 1;
        }

        return -1;
    }
}
