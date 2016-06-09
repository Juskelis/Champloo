﻿using UnityEngine;
using System.Collections;

public class OnWall : MovementState
{
    [SerializeField] private float maxFallSpeed = 10f;
    [SerializeField]
    private Vector2 wallJumpClimbForces;
    [SerializeField]
    private Vector2 wallJumpOffForces;
    [SerializeField]
    private Vector2 wallLeapForces;
    [SerializeField]
    private float wallStickTime;
    private float timeToWallUnstick;

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        int wallDirX = (controller.collisions.left) ? -1 : 1;
        velocity.x = input.leftStick.x;
        if (timeToWallUnstick > 0)
        {
            velocity.x = 0;

            if (input.leftStick.x != 0 && input.leftStick.x != wallDirX)
            {
                timeToWallUnstick -= Time.deltaTime;
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
        else
        {
            timeToWallUnstick = wallStickTime;
        }

        velocity.y -= player.Gravity * Time.deltaTime;
        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        DecayExternalForces(ref externalForces);

        bool jumped = false;
        if (input.jump.Down)
        {
            jumped = true;
            input.jump.ResetTimers();

            if (wallDirX == input.leftStick.x)
            {
                velocity.x = -wallDirX * wallJumpClimbForces.x;
                velocity.y = wallJumpClimbForces.y;
            }
            else if (input.leftStick.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOffForces.x;
                velocity.y = wallJumpOffForces.y;
            }
            else
            {
                velocity.x = -wallDirX * wallLeapForces.x;
                velocity.y = wallLeapForces.y;
            }
        }

        controller.Move(velocity*Time.deltaTime + externalForces * Time.deltaTime);

        if (controller.collisions.below)
        {
            return GetComponent<OnGround>();
        }
        else if (jumped || !(controller.collisions.left || controller.collisions.right))
        {
            return GetComponent<InAir>();
        }

        return null;
    }

    public override void OnEnter()
    {
        timeToWallUnstick = wallStickTime;
    }

    public override void OnExit()
    {
    }
}