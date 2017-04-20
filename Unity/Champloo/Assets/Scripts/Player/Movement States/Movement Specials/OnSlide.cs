using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSlide : OnMovementSpecial
{
    [SerializeField]
    private Vector2 wallSlideForce;
    [SerializeField]
    private float wallSlideUpDecay;
    [SerializeField]
    private float wallSlideDownDecay;
    [SerializeField]
    private Vector2 groundSlideForce;
    [SerializeField]
    private float groundSlideDecay;
    
    private bool goingRight = true;

    private bool goingUp = true;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        //if not sliding or in the air
        if (timingState != TimingState.IN_PROGRESS || GetSimulatedState() == GetComponent<InAir>())
        {
            return GetSimulatedState().ApplyFriction(velocity);
        } 
        if(GetSimulatedState() == GetComponent<OnWall>())
        {
            velocity.y += (goingUp ? wallSlideUpDecay: -wallSlideDownDecay);
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            velocity.x += (goingRight ? groundSlideDecay : -groundSlideDecay);
        }
        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (timingState == TimingState.DONE)
        {
            return GetSimulatedState();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        goingRight = inVelocity.x > 0;
        goingUp = inVelocity.y > 0;
        //if onWall
        if (GetSimulatedState() == GetComponent<OnWall>())
        {
            outVelocity.y = goingUp ? wallSlideForce.x : -wallSlideForce.x;
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            outVelocity.x = goingRight ? groundSlideForce.x : -groundSlideForce.x;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnEnd()
    {
        base.OnEnd();
    }
}
