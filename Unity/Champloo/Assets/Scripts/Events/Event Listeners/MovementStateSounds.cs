using System;
using TypeReferences;
using UnityEngine;

public class MovementStateSounds : MonoBehaviour
{
    [ClassExtends(typeof(MovementState))]
    public ClassTypeReference movementState;

    [SerializeField]
    private PlayRandomSource sound;

    [SerializeField]
    private bool loop = true;

    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnChange);
    }

    void OnDestroy()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        if (dispatcher != null)
        {
            dispatcher.RemoveListener<MovementStateChangedEvent>(OnChange);
        }
    }

    private void OnChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        if (e.Next.GetType() == movementState.Type)
        {
            sound.PlayLooped();
        }
        else
        {
            sound.Stop();
        }
    }
}
