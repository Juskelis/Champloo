using System;
using TypeReferences;
using UnityEngine;

public class MovementSpecialListener : MonoBehaviour {

    [SerializeField]
    private Transform toSpawnOnStart;

    [SerializeField]
    private Transform toSpawnOnEnd;

    [SerializeField]
    [ClassExtends(typeof(OnMovementSpecial))]
    private ClassTypeReference type;

    [SerializeField]
    private bool usePlayerDirection = true;

    private Player ourPlayer;

    private void Start()
    {
        GetComponent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnTiming);
        ourPlayer = GetComponentInParent<Player>();
    }

    private void OnTiming(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent)args;
        if (toSpawnOnStart != null && IsSubclassOrType(e.Next.GetType(), type.Type))
        {
            SpawnStartObject();
        }
        else if (toSpawnOnEnd != null && IsSubclassOrType(e.Previous.GetType(), type.Type))
        {
            SpawnEndObject();
        }
    }

    private void SpawnStartObject()
    {
        Transform spawned = Instantiate(toSpawnOnStart);
        if (usePlayerDirection)
        {
            Vector3 localScale = spawned.localScale;
            localScale.x = ourPlayer.HorizontalDirection;
            spawned.localScale = localScale;
        }
        spawned.SetParent(transform, false);
    }

    private void SpawnEndObject()
    {
        Transform spawned = Instantiate(toSpawnOnEnd);
        if (usePlayerDirection)
        {
            Vector3 localScale = spawned.localScale;
            localScale.x = ourPlayer.HorizontalDirection;
            spawned.localScale = localScale;
        }
        spawned.SetParent(transform, false);
    }

    private bool IsSubclassOrType(Type toCheck, Type target)
    {
        return toCheck == target || toCheck.IsSubclassOf(target);
    }
}
