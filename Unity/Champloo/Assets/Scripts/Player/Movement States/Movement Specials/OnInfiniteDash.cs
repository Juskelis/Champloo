using System;
using UnityEngine;
using System.Collections;

public class OnInfiniteDash : OnDash
{
    private bool hitDashEarly;

    protected override void Start()
    {
        base.Start();
        hitDashEarly = false;
        AttackAllowed = false;
    }
   
    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;
        //Buffer for attacks and movement specials done too early

        
        AttackAllowed = (attackBufferWindow > 0 && specialTimeLeft < (specialTime * attackBufferWindow) && onStartCalled);
        
        if (specialTimeLeft < (specialTime * nextDashBufferWindow))
        {
            if (player.InputPlayer.GetButtonDown("Movement Special") && !hitDashEarly)
            {
                hitNextDashInput = true;
                currentDashes++;
            }
        }
        else if (player.InputPlayer.GetButtonDown("Movement Special"))
        {
            hitDashEarly = true;
        }

        if(AttackAllowed && player.Weapon.IsAttacking)
        {
            return GetComponent<InAttack>();
        }

        if (isDisabled || timingState == TimingState.RECHARGE || timingState == TimingState.DONE)
        {
            hitDashEarly = false;
            if (hitNextDashInput)
            {
                hitNextDashInput = false;
                return GetComponent<OnInfiniteDash>();
            }
            else if (controller.collisions.Below)
            {
                return GetComponent<OnGround>();
            }
            else if (controller.collisions.Left || controller.collisions.Right)
            {
                return GetComponent<OnWall>();
            }
            return GetComponent<InAir>();
        }

        return null;
    }

    protected override void OnStateChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        if (e.Next is OnGround)
        {
            currentDashes = DashLimit;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        AttackAllowed = false;
    }
}