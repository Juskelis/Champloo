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
    private float wallSlideUpDecay;
    [SerializeField]
    private float wallSlideDownDecay;
    [SerializeField]
    private Vector2 groundSlideForce;
    [SerializeField]
    private float groundSlideDecay;

    [SerializeField]
    private PlayRandomSource wallSlideSound;

    private bool goingRight;

    private bool goingUp;

    private Vector3 appliedForce;

    private Vector2 test;

    private bool isOnWall;
    protected override void Start()
    {
        base.Start();
        goingUp = false;
        goingRight = false;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (isDisabled || !isInUse )
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }

        test = player.AimDirection;

        goingRight = velocity.x > 0;
        goingUp = velocity.y > 0;
        if (velocity.x == 0) goingRight = player.AimDirection.x > 0;

        if (GetSimulatedState() == GetComponent<OnWall>())
        {
            velocity.x = 0;
            goingRight = controller.collisions.Left;
            //decrease in speed going up, increase going down
            velocity.y += (goingUp ? -wallSlideUpDecay : -wallSlideDownDecay);
            
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            isOnWall = false;
            //if the player hits something while going horizontally
            if (controller.collisions.Right && velocity.x > 0)
            {
                velocity.y += velocity.x;
            }
            else if (controller.collisions.Left && velocity.x < 0)
            {
                velocity.y += -velocity.x;
            }
            //if no horizontal velocity, but there is negative vertical, assume the player is sliding down a wall
            else if (controller.collisions.Right && velocity.x == 0 && velocity.y < 0)
            {
                velocity.x += velocity.y;
                velocity.y = 0;
            }
            else if (controller.collisions.Left && velocity.x == 0 && velocity.y < 0)
            {
                velocity.x -= velocity.y;
                velocity.y = 0;
            }
            else
            {
                //add in any potential -y velocity if there is any, otherwise just decay the speed
                velocity.x += (goingRight ? (groundSlideDecay - velocity.y) : -(groundSlideDecay - velocity.y));
                //velocity.y = 0;
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
        //short out
        if(GetSimulatedState() == GetComponent<InAir>())
        {
            timingState = TimingState.DONE;
            outVelocity = inVelocity;
            outExternalForces = inExternalForces;
            return;
        }
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        goingUp = player.Velocity.y > 0;
        goingRight = player.Velocity.x > 0;
    }


    protected override void OnStart()
    {
        //if player has somehow entered the air since the OnEnter method, short out again
        if (GetSimulatedState() == GetComponent<InAir>())
        {
            timingState = TimingState.DONE;
            return;
        }

        base.OnStart();

        test = player.AimDirection;

        goingUp = player.Velocity.y > 0;
        goingRight = player.Velocity.x > 0;

        //if a player isn't moving, get their aim direction/direction they are facing, otherwise you can only slide the way you are running
        if (player.Velocity.x == 0) goingRight = player.AimDirection.x > 0;

        
        if (GetSimulatedState() == GetComponent<OnWall>())
        {
            appliedForce.y = goingUp ? wallSlideUpForce : -wallSlideDownForce;
        }
        else if (GetSimulatedState() == GetComponent<OnGround>())
        {
            appliedForce.y = 0;
            appliedForce.x = goingRight ? groundSlideForce.x : -groundSlideForce.x;
        }

        player.ApplyForce(appliedForce);
    }
}
