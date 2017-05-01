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

    [SerializeField]
    private PlayRandomSource wallSlideSound;

    private bool goingRight = true;

    private bool goingUp = true;


    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        //if not sliding or in the air
        if (timingState != TimingState.IN_PROGRESS || GetSimulatedState() == GetComponent<InAir>())
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }

        goingRight = velocity.x > 0;
        if (velocity.x == 0) goingRight = player.AimDirection.x > 0;
        if (GetSimulatedState() == GetComponent<OnWall>())
        {
            //if sliding down and hit ground;
            //this may not work as GetSimulatedState may return OnGround before this, so we'll need to change if that is the case
            if (!goingUp && controller.collisions.Below)
            {
                Debug.Log("on wall, detected collison below");
                if (controller.collisions.Left)
                {
                    goingRight = true;
                    velocity.x += velocity.y;
                }
                else if(controller.collisions.Right)
                {
                    goingRight = false;
                    velocity.x -= velocity.y;
                }
            }
            else
            {
                velocity.y += (goingUp ? wallSlideUpDecay : -wallSlideDownDecay);
            }
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            //if the player hits something 
            if (controller.collisions.Right && goingRight)
            {
                velocity.y += velocity.x;
            }
            else if (controller.collisions.Left && !goingRight)
            {
                velocity.y += -velocity.x;
            }
            else
            {
                //add in any potential -y velocity if there is any, otherwise just decay the speed

                velocity.x += (goingRight ? (groundSlideDecay - velocity.y) : -(groundSlideDecay - velocity.y));
                velocity.y = 0;
            }
        }
        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (timingState == TimingState.DONE)
        {
            return GetSimulatedState();
        }
        //if InAir, or hit something from below
        else if(GetSimulatedState() == GetComponent<InAir>() || (GetSimulatedState() == GetComponent<OnWall>() && controller.collisions.Above))
        {
            return GetComponent<InAir>();
        }


        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        goingRight = inVelocity.x > 0;
        //if a player isn't moving, get their aim direction/direction they are facing, otherwise you can only slide the way you are running
        if (inVelocity.x == 0) goingRight = player.AimDirection.x > 0; 
        goingUp = inVelocity.y > 0;
        //if onWall
        if (GetSimulatedState() == GetComponent<OnWall>())
        {
            outVelocity.y = goingUp ? wallSlideForce.y : -wallSlideForce.y;
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            outVelocity.x = goingRight ? groundSlideForce.x : -groundSlideForce.x;
        }
    }
}
