using System;
using TypeReferences;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MovementStateFootDust : MonoBehaviour
{
    [ClassExtends(typeof(MovementState))]
    public ClassTypeReference movementState;

    [SerializeField]
    public Transform jumpPuff;

    private ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        dispatcher.AddListener<MovementStateChangedEvent>(OnChange);
        dispatcher.AddListener<JumpEvent>(OnJump);
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

    private void OnJump(object sender, EventArgs args)
    {
        JumpEvent e = (JumpEvent) args;
        if (e.Active.GetType() == movementState.Type)
        {
            Instantiate(jumpPuff, transform.position, Quaternion.identity);
        }
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
