﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRoll : OnMovementSpecial
{
    [SerializeField]
    private float rollSpeed = 1f;
    [SerializeField]
    private LayerMask passthroughMask;

    private LayerMask originalCollisionMask;
    private LayerMask originalCrushMask;

    private bool goingRight = true;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (timingState != TimingState.IN_PROGRESS)
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }

        return (goingRight ? Vector3.right : Vector3.left) * rollSpeed;
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
    }

    protected override void OnStart()
    {
        base.OnStart();

        originalCollisionMask = controller.collisionMask;
        originalCrushMask = controller.crushMask;

        controller.collisionMask = passthroughMask;
        controller.crushMask = passthroughMask;
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        controller.collisionMask = originalCollisionMask;
        controller.crushMask = originalCrushMask;
    }
}