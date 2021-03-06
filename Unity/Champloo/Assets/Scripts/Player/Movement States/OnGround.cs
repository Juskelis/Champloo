﻿using UnityEngine;
using System.Collections;

public class OnGround : MovementState
{
    [SerializeField]
    private float jumpWindow = 0.1f;

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

    public float MaxSpeed { get { return maxSpeed; } }

    private float jumpVelocity;
    public float JumpVelocity { get {return jumpVelocity; } }

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;

    public bool Jumped { get; private set; }

    protected override void Start()
    {
        base.Start();
        //maxJumpVelocity = player.Gravity * player.timeToJumpApex;

        jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(player.Gravity) * player.maxJumpHeight);

        //vT = v0 + at
        acceleration = maxSpeed / stopToMaxSpeedTime;
        deceleration = maxSpeed / maxSpeedToStopTime;
        turningDeceleration = (maxSpeed * 2) / fullTurnTime;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        float moveX = inputController.ApplyDeadZone(input.GetAxis("Move Horizontal"));
        if (Mathf.Abs(moveX) <= float.Epsilon)
        {
            //stopping
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }
        return velocity;
    }

    public Vector3 OnJump(Vector3 inVelocity = default(Vector3))
    {
        Vector3 outVelocity = inVelocity;
        Jumped = true;
        inputController.ConsumeButton("Jump");
        player.FireEvent(new JumpEvent { Active = this, Direction = Vector3.up });
        outVelocity.y = jumpVelocity;
        return outVelocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        Jumped = false;
        if (inputController.IsDown("Jump", jumpWindow))
        {
            outVelocity = OnJump();
        }

        float moveX = inputController.ApplyDeadZone(input.GetAxis("Move Horizontal"));
        float inputDirection = Mathf.Abs(moveX) > float.Epsilon ? Mathf.Sign(moveX) : 0f;
        if (Mathf.Abs(moveX) >= float.Epsilon)
        {
            float delta = inputDirection != Mathf.Sign(inVelocity.x) ? turningDeceleration : acceleration;
            outVelocity.x = Mathf.MoveTowards(
                inVelocity.x,
                maxSpeed*(analogMovementSpeed ? moveX : inputDirection),
                delta*Time.deltaTime);
        }
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (!controller.collisions.Below || Jumped)
        {

            if (controller.collisions.Left || controller.collisions.Right)
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
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        outVelocity.y = 0;
        Jumped = false;
    }

}
