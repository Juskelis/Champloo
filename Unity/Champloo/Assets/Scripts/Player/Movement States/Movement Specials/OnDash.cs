﻿using UnityEngine;
using System.Collections;

public class OnDash : OnMovementSpecial
{
    [SerializeField]
    private float dashForce;
    public float DashForce { get { return dashForce; } }

    //private float timeLeft;

    [SerializeField]
    private float gravityModifier;

    [SerializeField]
    private int dashLimit;
    public int DashLimit { get { return dashLimit; } }

    [SerializeField]
    private PlayRandomSource dashSound;
    
    private int currentDashes;

    private Vector2 direction;
    private Vector3 dashVelocity;

    private bool earlyAttackInput;
    private bool earlyDashInput;
    private bool justStarted;
    TrailRenderer tail;

    protected override void Start()
    {
        base.Start();
        currentDashes = dashLimit;
        tail = GetComponent<TrailRenderer>();
        earlyAttackInput = false;
        earlyDashInput = false;
    }

    // simulate inair while we are starting up or cooling down
    private Vector3 ApplyFrictionInactive(Vector3 velocity)
    {
        if ((controller.collisions.Below && velocity.y <= 0)
            || (controller.collisions.Above && velocity.y >= 0))
        {
            velocity.y = 0;
        }

        velocity.y -= player.Gravity * Time.deltaTime;
        if (velocity.y < -dashForce) velocity.y = -dashForce;

        if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
        {
            velocity.x = 0;
        }

        return velocity;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (isDisabled || timingState != TimingState.IN_PROGRESS) return ApplyFrictionInactive(velocity);
        if (justStarted) return direction;

        velocity.y -= player.Gravity * Time.deltaTime * gravityModifier;

        //check for collisions in the x direction that the player is dashing
        if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
        {
            velocity.x = 0;
        }

        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;

        //Buffer for attacks and movement specials done too early
        if (specialTimeLeft < (specialTime / 5))
        {
            if (player.InputPlayer.GetButtonDown("Movement Special"))
            {
                earlyAttackInput = true;
            }
            if (player.InputPlayer.GetButtonDown("Movement Special"))
            {
                earlyDashInput = true;
            }
        }

        if (isDisabled || timingState == TimingState.DONE)
        {
            if(earlyAttackInput)
            {
                return GetComponent<InAttack>();
            }
            else if(earlyDashInput)
            {
                return GetComponent<OnDash>();
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

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        //short out
        if (currentDashes <= 0)
        {
            timingState = TimingState.DONE;

            outVelocity = inVelocity;
            outExternalForces = inExternalForces;

            return;
        }

        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        //Establish the direction of the dash
        direction = player.AimDirection;

        currentDashes--;
        isDisabled = false;

        //actually apply dash forces
        direction = (direction == Vector2.zero ? Vector2.up : direction) * DashForce;
        dashSound.Play();
        justStarted = false;
    }

    protected override void OnStart()
    {
        base.OnStart();
        justStarted = true;

        specialTimeLeft = specialTime;

        //set up the dash visual trail
        tail.enabled = true;
        tail.Clear();
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        earlyAttackInput = false;
        earlyDashInput = false;
        tail.enabled = false;
    }
    
    //Functions to be called on state changes

    //ground state changes
    public override void OnEnterGround(Vector3 velocity, Vector3 externalForces)
    {
        currentDashes = DashLimit;
    }

    //wallride state changes
    public override void OnEnterWall(Vector3 velocity, Vector3 externalForces)
    {
        if (currentDashes < 1)
        {
            currentDashes = 1;
        }
    }
}
