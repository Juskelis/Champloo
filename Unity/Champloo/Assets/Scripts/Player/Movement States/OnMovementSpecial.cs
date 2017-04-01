using UnityEngine;
using System.Collections;

public class OnMovementSpecial : MovementState
{
    protected TimingState timingState;
    public TimingState Progress { get { return timingState; } }

    [SerializeField]
    protected float startupTime;
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float cooldownTime;
    [SerializeField]
    protected float rechargeTime;


    protected float specialTimeLeft;
    protected float cooldownTimeLeft;

    //bools for determining current state
    public bool canUse { get { return !isDisabled && timingState == TimingState.DONE; } }
    public bool isInUse { get { return timingState == TimingState.IN_PROGRESS; } }
    public bool isDisabled { get; set; }

    protected override void Start()
    {
        base.Start();
        isDisabled = false;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        StartCoroutine(TimingCoroutine());
    }

    private IEnumerator TimingCoroutine()
    {
        timingState = TimingState.WARMUP;
        yield return new WaitForSeconds(startupTime);
        OnStart();
        yield return new WaitForSeconds(specialTime);
        OnEnd();
        yield return new WaitForSeconds(cooldownTime);
        OnCooledDown();
        yield return null;
    }

    protected virtual IEnumerator RechargeCoroutine()
    {
        yield return null;
    }

    protected virtual void OnStart()
    {
        timingState = TimingState.IN_PROGRESS;
    }

    protected virtual void OnEnd()
    {
        timingState = TimingState.COOLDOWN;
    }

    protected virtual void OnCooledDown()
    {
        timingState = TimingState.DONE;
    }

    protected virtual void OnRecharge()
    {

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
