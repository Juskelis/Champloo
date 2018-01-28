using System;
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
    public virtual bool canUse { get { return !isDisabled && timingState == TimingState.DONE; } }
    public virtual bool isInUse { get { return timingState == TimingState.IN_PROGRESS; } }
    public virtual bool isDisabled { get; set; }

    //Used On the start of the player object
    protected override void Start()
    {
        base.Start();
        isDisabled = false;
        timingState = TimingState.DONE; 
        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnStateChange);
        StopAllCoroutines();
    }

    public override void Reset()
    {
        base.Reset();
        timingState = TimingState.DONE;
        StopAllCoroutines();
    }

    //Used on the entering of the movement special, before the warmup
    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        StartCoroutine(TimingCoroutine());
    }

    public override bool AttackAllowed { get { return false; } }

    protected virtual IEnumerator TimingCoroutine()
    {
        timingState = TimingState.WARMUP;
        player.FireEvent(new MovementSpecialTimingEvent { Special = this, Timing = timingState });
        yield return new WaitForSeconds(startupTime);
        OnStart();
        if (specialTime > 0)
        {
            yield return new WaitForSeconds(specialTime);
        }
        OnEnd();
        if (cooldownTime > 0)
        {
            yield return new WaitForSeconds(cooldownTime);
        }
        OnCooledDown();
        if (rechargeTime > 0)
        {
            yield return new WaitForSeconds(rechargeTime);
        }
        OnRecharge();
    }

    protected virtual IEnumerator RechargeCoroutine()
    {
        yield return null;
    }

    //used on the start of the move, after the warmup
    protected virtual void OnStart()
    {
        timingState = TimingState.IN_PROGRESS;
        player.FireEvent(new MovementSpecialTimingEvent {Special = this, Timing = timingState});
    }

    //used on the end of the move, before the cooldown
    protected virtual void OnEnd()
    {
        timingState = TimingState.COOLDOWN;
        player.FireEvent(new MovementSpecialTimingEvent { Special = this, Timing = timingState });
    }

    //used at the end of the cooldown period
    protected virtual void OnCooledDown()
    {
        timingState = TimingState.RECHARGE;
        player.FireEvent(new MovementSpecialTimingEvent { Special = this, Timing = timingState });
    }

    //used at the end of the recharge period, when the move is available to be used again
    protected virtual void OnRecharge()
    {
        timingState = TimingState.DONE;
        player.FireEvent(new MovementSpecialTimingEvent { Special = this, Timing = timingState });
    }

    /*
        Function to be called on state changes if necessary
        Shouldn't modify state. If necessary, do so through player.OnVelocityChanged(newVelocity)
        and document it
    */
    protected virtual void OnStateChange(object sender, EventArgs args)
    {

    }
}
