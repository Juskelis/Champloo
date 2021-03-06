﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRoll : OnMovementSpecial
{
    [SerializeField]
    private Vector2 rollSpeed;

    [SerializeField]
    private LayerMask rollingCollisionMask;
    [SerializeField]
    private LayerMask rollingCrushMask;
    [SerializeField]
    private LayerMask rollingNotifyMask;

    private LayerMask originalCollisionMask;
    private LayerMask originalCrushMask;
    private LayerMask originalNotifyMask;

    private bool goingRight = true;
    private bool canAirRoll = true;

    private bool firstFrame = false;

    public override bool canUse
    {
        get { return base.canUse && (!(GetSimulatedState() is InAir) || canAirRoll); }
    }


    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (timingState != TimingState.IN_PROGRESS)
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }

        if (firstFrame)
        {
            firstFrame = false;
            Vector3 retVector = goingRight ? Vector3.right : Vector3.left;
            retVector.x *= rollSpeed.x;
            retVector.y = rollSpeed.y;
            return retVector;
        }

        Vector3 modifiedVector = velocity.x > 0 ? Vector3.right : Vector3.left;
        modifiedVector.x *= rollSpeed.x;
        modifiedVector.y = rollSpeed.y;
        return modifiedVector;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (timingState == TimingState.RECHARGE ||  timingState == TimingState.DONE)
        {
            return GetSimulatedState();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        //short out
        if(GetSimulatedState() is InAir)
        {
            if(canAirRoll)
            {
                canAirRoll = false;
            }
            else 
            {
                timingState = TimingState.DONE;
                outVelocity = inVelocity;
                outExternalForces = inExternalForces;
                return;
            }
        }

        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        goingRight = player.AimDirection.x > 0;
        firstFrame = true;
    }

    protected override void OnStart()
    {
        base.OnStart();

        originalCollisionMask = controller.collisionMask;
        originalCrushMask = controller.crushMask;
        originalNotifyMask = controller.notifyMask;

        controller.collisionMask = rollingCollisionMask;
        controller.crushMask = rollingCrushMask;
        controller.notifyMask = rollingNotifyMask;

        player.OnInvicibleChanged(true);

        foreach (SpriteRenderer sprite in player.ColoredSprites)
        {
            sprite.color = Color.white;
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        controller.collisionMask = originalCollisionMask;
        controller.crushMask = originalCrushMask;
        controller.notifyMask = originalNotifyMask;

        player.OnInvicibleChanged(false);
        firstFrame = false;

        foreach (SpriteRenderer sprite in player.ColoredSprites)
        {
            sprite.color = player.PlayerColor;
        }
    }

    protected override void OnStateChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        if (e.Next is OnGround || e.Next is OnWall)
        {
            canAirRoll = true;
        }
    }
}
