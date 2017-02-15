using UnityEngine;
using System.Collections;

public class OnMovementSpecial : MovementState
{
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float cooldownTime;


    protected float specialTimeLeft;
    protected float cooldownTimeLeft;

    //bools for determining current state
    public bool canUse { get { return !isInUse && !isDisabled && cooldownTimeLeft > 0; } }
    public bool isInUse { get { return specialTimeLeft > 0; } }
    public bool isDisabled { get; set; }

    protected override void Start()
    {
        base.Start();
        isDisabled = false;
    }

    /*
        Functions to be called on state changes if necessary
        Shouldn't modify state. If necessary, do so through player.OnVelocityChanged(newVelocity)
        and document it
    */

    //ground state changes
    public virtual void OnEnterGround(Vector3 velocity, Vector3 externalForces) {}
    public virtual void OnExitGround(Vector3 velocity, Vector3 externalForces){}

    //air state changes
    public virtual void OnEnterAir(Vector3 velocity, Vector3 externalForces) { }
    public virtual void OnExitAir(Vector3 velocity, Vector3 externalForces) { }

    //wallride state changes
    public virtual void OnEnterWall(Vector3 velocity, Vector3 externalForces) { }
    public virtual void OnExitWall(Vector3 velocity, Vector3 externalForces) { }

    //attack state changes
    public virtual void OnEnterAttack(Vector3 velocity, Vector3 externalForces) { }
    public virtual void OnExitAttack(Vector3 velocity, Vector3 externalForces) { }

    //block state changes
    public virtual void OnEnterBlock(Vector3 velocity, Vector3 externalForces) { }
    public virtual void OnExitBlock(Vector3 velocity, Vector3 externalForces) { }

    //taunt state changes
    public virtual void OnEnterTaunt(Vector3 velocity, Vector3 externalForces) { }
    public virtual void OnExitTaunt(Vector3 velocity, Vector3 externalForces) { }

    //May want to add in a bounce state later for when you bounce off of an opponent's head
}
