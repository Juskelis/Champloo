using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSlide : OnMovementSpecial
{

    [SerializeField]
    private float wallSlideUpForce;
    [SerializeField]
    private float wallSlideDownForce;
    [SerializeField]
    private float wallSlideUpMod;
    [SerializeField]
    private float wallSlideDownMod;
    [SerializeField]
    private float groundSlideForce;
    [SerializeField]
    private float groundSlideMod;


    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        MovementState simulatedState = GetSimulatedState();
        if (isDisabled || !isInUse )
        {
            return simulatedState.ApplyFriction(velocity);
        }
        bool goingRight = velocity.x > 0;
        bool goingUp = velocity.y > 0;
        if (velocity.x == 0) goingRight = player.AimDirection.x > 0;

        if (simulatedState is OnWall)
        {
            velocity.x = 0;
            //decrease in speed going up, increase going down
            velocity.y += (goingUp ? -wallSlideUpMod: -wallSlideDownMod);
        }
        else if (simulatedState is OnGround)
        {
            if(!controller.collisions.Left && !controller.collisions.Right)
            {
                //add in any potential -y velocity if there is any, otherwise just decay the speed
                velocity.x += (goingRight ? (groundSlideMod - velocity.y) : -(groundSlideMod - velocity.y));
                //velocity.y = 0;
            }
            //if the player hits something while going horizontally
            else if (controller.collisions.Right)
            {
                if (velocity.x > 0)
                {
                    velocity.y += velocity.x;
                }
                //if no horizontal velocity, but there is negative vertical, assume the player is sliding down a wall
                else if (velocity.x == 0 && velocity.y < 0)
                {
                    velocity.x += velocity.y;
                    velocity.y = 0;
                }
            }
            else if (controller.collisions.Left)
            {
                if(velocity.x < 0)
                {
                    velocity.y += -velocity.x;
                }
                else if(velocity.x == 0 && velocity.y < 0)
                {
                    velocity.x -= velocity.y;
                    velocity.y = 0;
                }
            }
        }
        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        MovementState simulatedState = GetSimulatedState();
        if (timingState == TimingState.DONE || simulatedState is InAir)
        {
            return simulatedState;
        }
        else if (simulatedState is OnWall && controller.collisions.Above)
        {
            return GetComponent<InAir>();
        }
        return null;
    }


    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        //short out
        if(GetSimulatedState() is InAir)
        {
            timingState = TimingState.DONE;
            outVelocity = inVelocity;
            outExternalForces = inExternalForces;
            return;
        }
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
    }


    protected override void OnStart()
    {
        Vector3 appliedForce = Vector3.zero;
        //if player has somehow entered the air since the OnEnter method, short out again
        MovementState simulatedState = GetSimulatedState();
        if (simulatedState is InAir)
        {
            timingState = TimingState.DONE;
            return;
        }

        base.OnStart();
        
        bool goingUp = player.Velocity.y > 0;
        bool goingRight = player.Velocity.x > 0;

        //if a player isn't moving, get their aim direction/direction they are facing, otherwise you can only slide the way you are running
        if (player.Velocity.x == 0) goingRight = player.AimDirection.x > 0;

        
        if (simulatedState is OnWall)
        {
            appliedForce.y = goingUp ? wallSlideUpForce : -wallSlideDownForce;
        }
        else if (simulatedState is OnGround)
        {
            appliedForce.y = 0;
            appliedForce.x = goingRight ? groundSlideForce : -groundSlideForce;
        }

        player.ApplyForce(appliedForce);
    }
}
