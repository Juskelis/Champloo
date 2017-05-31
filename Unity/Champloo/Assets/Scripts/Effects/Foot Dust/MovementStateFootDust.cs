using System;
using TypeReferences;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MovementStateFootDust : MonoBehaviour
{
    [ClassExtends(typeof(MovementState))]
    public ClassTypeReference movementState;

    [SerializeField]
    private Transform leaveParticle;

    [SerializeField]
    private Transform enterParticle;

    private ParticleSystem particle;
    private Player p;

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
            if(!particle.isEmitting) SetPlaying(true);
        }
        else
        {
            if(particle.isEmitting) SetPlaying(false);
        }
    }

    void SetPlaying(bool playing)
    {
        if (!playing)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            if(leaveParticle != null) Instantiate(leaveParticle, transform.position, Quaternion.identity);
        }
        else
        {
            particle.Play(true);
            if (enterParticle != null) Instantiate(enterParticle, transform.position, Quaternion.identity);
        }
    }
}
