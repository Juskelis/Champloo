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

    [SerializeField]
    private Vector2 wallJumpVelocity;


    private Vector2 direction;

    //Wallride variables being made accessable

    public Vector2 WallJumpClimbForces { get { return wallJumpClimbForces; } }
    public Vector2 WallJumpOffForces { get { return wallJumpOffForces; } }
    public Vector2 WallLeapForces { get { return wallLeapForces; } }
    public float WallStickTime { get { return wallStickTime; } }

    [SerializeField]
    private PlayRandomSource wallSlideSound;
    [SerializeField]
    private PlayRandomSource jumpSound;

    private float timeToWallUnstick;

    private bool jumped = false;
    protected override void Start()
    {
        base.Start();
    }


    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        if (!jumped)
        {
            if (timeToWallUnstick > 0)
            {
                velocity.x = 0;

                if (moveX != 0 && Mathf.Sign(moveX) != Mathf.Sign(wallDirX))
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
                //actively speed up until off the wall
                velocity.x = moveX * (2 + Mathf.Abs(timeToWallUnstick));
            }

            velocity.y -= player.Gravity * Time.deltaTime;
            if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        }
        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        //normalize the direction so that it just gives the wall jump angle
        direction = player.AimDirection.normalized;
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        //float moveX = player.InputPlayer.GetAxis("Move Horizontal");

        jumped = false;
        if (player.InputPlayer.GetButtonDown("Jump"))
        {

            Debug.Log("direction x: " + direction.x + " direction y: " + direction.y);
            jumped = true;
            jumpSound.Play();

            //PLayer will always jump off of a wall a little bit
            outVelocity.x = (wallJumpVelocity.x / 2) * (-wallDirX) ;

            //if the input is pointed towards the wall
            if (Mathf.Abs(wallDirX - direction.x) < 0.5f)
            {
                outVelocity.x = (-wallDirX) * (wallJumpVelocity.x / 2);
                outVelocity.y = wallJumpVelocity.y;
            }
            //if player has no input
            else if(direction.x == 0)
            {
                outVelocity.x = -wallDirX * wallJumpVelocity.x;
                outVelocity.y = wallJumpVelocity.y * .5f;

            }
            //If player has directional input
            else
            {
                outVelocity.x = wallJumpVelocity.x * direction.x;
                outVelocity.y = wallJumpVelocity.y * direction.y;

            }


            //if the input is pointed towards the wall
            //if (Mathf.Abs(wallDirX - moveX) < 0.5f)
            //{
            //    outVelocity.x = -wallDirX * wallJumpClimbForces.x;
            //    outVelocity.y = wallJumpClimbForces.y;
            //}
            ////if the player has no x input
            //else if (moveX == 0)
            //{
            //    outVelocity.x = -wallDirX * wallJumpOffForces.x;
            //    outVelocity.y = wallJumpOffForces.y;
            //}
            ////assuming that the player is pointing away from the wall
            //else
            //{
            //    outVelocity.x = -wallDirX * wallLeapForces.x;
            //    outVelocity.y = wallLeapForces.y;
            //}
        }

        if (!wallSlideSound.Playing)
        {
            wallSlideSound.Play();
        }
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }

        if (jumped || !(controller.collisions.Left || controller.collisions.Right))
        {
            return GetComponent<InAir>();
        }

        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        timeToWallUnstick = wallStickTime;
        movementSpecial.OnEnterWall(inVelocity, inExternalForces);
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitWall(inVelocity, inExternalForces);
        jumped = false;
        wallSlideSound.Stop();
    }
}
