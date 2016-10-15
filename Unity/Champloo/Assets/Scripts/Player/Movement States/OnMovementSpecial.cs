using UnityEngine;
using System.Collections;

public class OnMovementSpecial : MovementState
{
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float cooldownTime;


    protected float specialTimer;
    protected float cooldownTimer;

    //bools for determining current state
    public bool canUse { get { return !isInUse && !isDisabled; } }
    public bool isInUse { get { return specialTime >= 0; } }
    public bool isDisabled { get; set; }

    protected override void Start()
    {
        base.Start();
        isDisabled = false;
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        return null;
    }

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
    }

    //Functions to be called on state changes if necessary

    //ground state changes
    public virtual void OnEnterGround(ref Vector3 velocity, ref Vector3 externalForces) {}
    public virtual void OnExitGround(ref Vector3 velocity, ref Vector3 externalForces){}

    //air state changes
    public virtual void OnEnterAir(ref Vector3 velocity, ref Vector3 externalForces) { }
    public virtual void OnExitAir(ref Vector3 velocity, ref Vector3 externalForces) { }

    //wallride state changes
    public virtual void OnEnterWall(ref Vector3 velocity, ref Vector3 externalForces) { }
    public virtual void OnExitWall(ref Vector3 velocity, ref Vector3 externalForces) { }

    //attack state changes
    public virtual void OnEnterAttack(ref Vector3 velocity, ref Vector3 externalForces) { }
    public virtual void OnExitAttack(ref Vector3 velocity, ref Vector3 externalForces) { }

    //block state changes
    public virtual void OnEnterBlock(ref Vector3 velocity, ref Vector3 externalForces) { }
    public virtual void OnExitBlock(ref Vector3 velocity, ref Vector3 externalForces) { }

    //taunt state changes
    public virtual void OnEnterTaunt(ref Vector3 velocity, ref Vector3 externalForces) { }
    public virtual void OnExitTaunt(ref Vector3 velocity, ref Vector3 externalForces) { }

    //May want to add in a bounce state later for when you bounce off of an opponent's head
}
