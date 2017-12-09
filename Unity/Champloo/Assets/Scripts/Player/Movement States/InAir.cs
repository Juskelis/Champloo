using System;
using UnityEngine;
using System.Collections;
using Rewired;

public class InAir : MovementState
{
    [SerializeField]
    private float maxFallSpeed = 20f;
    public float MaxFallSpeed { get { return maxFallSpeed; } }

    [SerializeField]
    private float maxSpeed = 6;
    [SerializeField]
    private float stopToMaxSpeedTime = 0.1f;
    [SerializeField]
    private float maxSpeedToStopTime = 0.2f;
    [SerializeField]
    private float fullTurnTime = 0.2f;

    [SerializeField]
    private bool analogMovementSpeed = false;

    [SerializeField]
    [Tooltip("How long into entering air can we still jump?")]
    private float coyoteTime = 0.2f;

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;
    private bool hasShortened;

    private float onGroundExitTime = Mathf.NegativeInfinity;
    private float onWallExitTime = Mathf.NegativeInfinity;

    protected override void Start()
    {
        base.Start();

        //vT = v0 + at
        acceleration = maxSpeed / stopToMaxSpeedTime;
        deceleration = maxSpeed / maxSpeedToStopTime;
        turningDeceleration = (maxSpeed * 2) / fullTurnTime;

        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnMovementStateChanged);
    }

    private void OnMovementStateChanged(object sender, EventArgs args)
    {
        MovementStateChangedEvent evt = (MovementStateChangedEvent) args;
        if (evt.Previous.GetType() == typeof(OnGround) && !((OnGround)evt.Previous).Jumped)
        {
            onGroundExitTime = Time.time;
        }
        if (evt.Previous.GetType() == typeof(OnWall) && !((OnWall)evt.Previous).Jumped)
        {
            onWallExitTime = Time.time;
        }
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        float moveX = inputController.ApplyDeadZone(input.GetAxis("Move Horizontal"));
        if (Mathf.Abs(moveX) <= float.Epsilon)
        {
            //stopping
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        float moveX = inputController.ApplyDeadZone(input.GetAxis("Move Horizontal"));
        float inputDirection = Mathf.Abs(moveX) > float.Epsilon ? Mathf.Sign(moveX) : 0f;
        if (Mathf.Abs(moveX) > float.Epsilon)
        {
            float delta = inputDirection != Mathf.Sign(inVelocity.x) ? turningDeceleration : acceleration;
            outVelocity.x = Mathf.MoveTowards(
                inVelocity.x,
                maxSpeed*(analogMovementSpeed ? moveX : inputDirection),
                delta*Time.deltaTime);
        }

        //if the player releases the jump button and is is moving up
        if (!player.InputPlayer.GetButton("Jump") && !hasShortened  && inVelocity.y > 0)
        {
            outVelocity.y = inVelocity.y / 2;
            hasShortened = true;
        }

        if (onGroundExitTime > 0 && Time.time - onGroundExitTime <= coyoteTime && inputController.IsDown("Jump"))
        {
            //coyote jump!
            hasShortened = false;
            outVelocity = GetComponentInParent<OnGround>().OnJump(outVelocity);
        }

        if (onWallExitTime > 0 && Time.time - onWallExitTime <= coyoteTime && inputController.IsDown("Jump"))
        {
            //coyote jump!
            hasShortened = false;
            outVelocity = GetComponentInParent<OnWall>().OnJump();
        }
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }
        else if (controller.collisions.Left || controller.collisions.Right)
        {
            return GetComponent<OnWall>();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnEnterAir(inVelocity, inExternalForces);
        hasShortened = false;
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitAir(inVelocity, inExternalForces);
    }
}
