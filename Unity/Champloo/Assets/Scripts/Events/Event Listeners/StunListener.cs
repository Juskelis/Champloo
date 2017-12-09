using System;
using UnityEngine;

public class StunListener : MonoBehaviour
{
    [SerializeField]
    private Transform toSpawnOnStart;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Transform toSpawnOnEnd;

    private Transform spawnedStunEffect;

    void Start()
    {
        GetComponent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnTiming);
        GetComponent<LocalEventDispatcher>().AddListener<DeathEvent>(OnDeath);
    }

    private void OnDeath(object sender, EventArgs args)
    {
        foreach (Transform t in Utility.FindAll(transform, toSpawnOnStart.name))
        {
            Destroy(t.gameObject);
        }
        spawnedStunEffect = null;
    }

    private void OnTiming(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent)args;
        if (e.Next is InStun)
        {
            if (toSpawnOnStart != null)
            {
                spawnedStunEffect = Instantiate(toSpawnOnStart);
                spawnedStunEffect.SetParent(transform, false);
                spawnedStunEffect.Translate(offset);
            }
        }
        else
        {
            if (spawnedStunEffect != null)
            {
                spawnedStunEffect.GetComponentInChildren<ParticleSystem>().Stop();
                spawnedStunEffect = null;
            }

            if (toSpawnOnEnd != null)
            {
                Transform spawned = Instantiate(toSpawnOnEnd);
                spawned.SetParent(transform, false);
            }
        }
    }
}
