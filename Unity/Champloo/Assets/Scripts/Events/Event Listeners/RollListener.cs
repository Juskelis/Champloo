using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class RollListener : MonoBehaviour {
    
    [SerializeField]
    private Transform toSpawnOnStart;

    [SerializeField]
    private Transform toSpawnOnEnd;

    void Start()
    {
        GetComponent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnTiming);
    }

    private void OnTiming(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent)args;
        if (e.Next is OnRoll)
        {
            Transform spawned = Instantiate(toSpawnOnStart);
            spawned.SetParent(transform, false);
        } else if (e.Previous is OnRoll)
        {
            Transform spawned = Instantiate(toSpawnOnEnd);
            spawned.SetParent(transform, false);
        }
    }
}
