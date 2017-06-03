using System;
using TypeReferences;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MovementStateFootDust : MonoBehaviour
{
    [ClassExtends(typeof(MovementState))]
    public ClassTypeReference movementState;

    private ParticleSystem particle;
    private Player p;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        p = GetComponentInParent<Player>();
        if (p == null)
        {
            throw new ArgumentNullException();
        }
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
        MovementStateChangedEvent convertedArgs = (MovementStateChangedEvent) args;
        togglePlaying(convertedArgs.Next.GetType() == movementState.Type);
    }

    void togglePlaying(bool playing)
    {
        if (!playing)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            particle.Play(true);
        }
    }
}
