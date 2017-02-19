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

    private bool playing = true;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        p = GetComponentInParent<Player>();
        if (p == null)
        {
            throw new ArgumentNullException();
        }
    }

    void Update()
    {
        if (p.CurrentMovementState.GetType() == movementState.Type)
        {
            if(!playing) SetPlaying(true);
        }
        else
        {
            if(playing) SetPlaying(false);
        }
    }

    void SetPlaying(bool playing)
    {
        this.playing = playing;
        ParticleSystem.EmissionModule module = particle.emission;
        module.enabled = playing;
    }
}
