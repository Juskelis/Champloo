using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Networking;


public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _dict;
    public ReadOnlyDictionary(IDictionary<TKey, TValue> dict)
    {
        if(dict == null) throw new System.ArgumentNullException();
        _dict = dict;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        //throw new System.NotImplementedException();
        return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotSupportedException();
    }

    public void Clear()
    {
        throw new System.NotSupportedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dict.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _dict.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new System.NotSupportedException();
    }

    public int Count { get { return _dict.Count; } private set {throw new System.NotSupportedException();} }
    public bool IsReadOnly { get { return _dict.IsReadOnly; } private set {throw new System.NotSupportedException();} }
    public void Add(TKey key, TValue value)
    {
        throw new System.NotSupportedException();
    }

    public bool ContainsKey(TKey key)
    {
        //throw new System.NotImplementedException();
        return _dict.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        throw new System.NotSupportedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        //throw new System.NotImplementedException();
        return _dict.TryGetValue(key, out value);
    }

    public TValue this[TKey key]
    {
        get { return _dict[key]; }
        set { throw new System.NotSupportedException(); }
    }

    public ICollection<TKey> Keys { get { return _dict.Keys; } private set {throw new System.NotSupportedException();} }
    public ICollection<TValue> Values { get { return _dict.Values; } private set {throw new System.NotSupportedException();} }
}


public class Score : NetworkBehaviour
{
    //public static List<int> Scores { get { return scores; } private set { scores = value; } }
    //private static List<int> scores;
    public static ReadOnlyDictionary<int, int> Scores
    {
        get
        {
            if(scores == null) scores = new Dictionary<int, int>();
            return new ReadOnlyDictionary<int, int>(scores);
        }
        private set {throw new System.NotSupportedException();}
    }
    private static Dictionary<int, int> scores; //key is NetworkID, val is score
    public static Score instance = null;

    [SerializeField]
    private int winScore = 7;

    public int WinScore
    {
        get { return winScore; }
    }

    //private List<int> scores;

    bool lateStartDone = false;

    [ServerCallback]
    private void Awake()
    {
        instance = this;
        if(scores == null)
            scores = new Dictionary<int, int>();

        scores.Clear();
    }

    //[Server]
    public static void AddPlayer(int networkID)
    {
        print("adding player: " + networkID);
        if(!scores.ContainsKey(networkID))
            scores.Add(networkID, 0);
    }

    [Server]
    public static void RemovePlayer(int networkID)
    {
        if(scores.ContainsKey(networkID))
            scores.Remove(networkID);
    }

    [ServerCallback]
    private void Update()
    {
        //moved score collection creation to player (players have to add themselves)
        //  HOWEVER, error checking for unadded players still exists
        /*
        if(!lateStartDone)
        {
            scores = new List<int>(FindObjectsOfType<Player>().Length);
            for (int i = 0; i < scores.Capacity; i++)
            {
                scores.Add(0);
            }
            lateStartDone = true;
        }
        */
    }

    [Command]
    public void CmdScore(int playerNumber)
    {
        if (!isServer) return;
        AddScore(playerNumber);
    }

    [Server]
    public void AddScore(int playerNumber)
    {
        if (!scores.ContainsKey(playerNumber))
        {
            Debug.LogWarning("Player with Network ID " + playerNumber + " scored w/o being added");
            AddPlayer(playerNumber);
        }

        scores[playerNumber]++;
        if (scores[playerNumber] >= winScore)
        {
            GetComponent<Match>().End();
        }
    }

    [Server]
    public void SubtractScore(int playerNumber)
    {
        if (!scores.ContainsKey(playerNumber))
        {
            Debug.LogWarning("Player with Network ID " + playerNumber + " de-scored w/o being added");
            AddPlayer(playerNumber);
        }

        if (scores[playerNumber] > 0)
            scores[playerNumber]--;
    }

    public bool IsTied()
    {
        int max = Scores.Values.Max();
        int shareMax = 0;
        foreach (int score in scores.Values)
        {
            if (score == max) shareMax++;
        }

        return shareMax > 1;
    }

    /// <summary>
    /// Find the index of the player in the lead. Assumes no ties, otherwise returns arbitrary tie-holder.
    /// </summary>
    /// <returns>The NetworkID of the winning player</returns>
    ///// <returns>The controller number (1 indexed) of the winning player</returns>
    public int FindWinner()
    {
        int max = scores.Values.Max();
        foreach (var pair in scores)
        {
            if (pair.Value >= max) return pair.Key;
        }
        /*
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] >= max) return i + 1;
        }
        */
        return -1;
    }
}
