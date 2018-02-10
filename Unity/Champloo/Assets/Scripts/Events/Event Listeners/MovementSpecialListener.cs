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

    private void Start()
    {
        GetComponent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnTiming);
    }

    private void OnTiming(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent)args;
        if (toSpawnOnStart != null && IsSubclassOrType(e.Next.GetType(), type.Type))
        {
            Transform spawned = Instantiate(toSpawnOnStart);
            spawned.SetParent(transform, false);
        }
        else if (toSpawnOnEnd != null && IsSubclassOrType(e.Previous.GetType(), type.Type))
        {
            Transform spawned = Instantiate(toSpawnOnEnd);
            spawned.SetParent(transform, false);
        }
    }

    private bool IsSubclassOrType(Type toCheck, Type target)
    {
        return toCheck == target || toCheck.IsSubclassOf(target);
    }
}
