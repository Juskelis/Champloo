using UnityEngine;
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
    private PlayRandomSource wallSlideSound;
    [SerializeField]
    private PlayRandomSource jumpSound;

    private float timeToWallUnstick;

    private bool jumped = false;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        velocity.x = moveX;
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

        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");

        jumped = false;
        if (player.InputPlayer.GetButtonDown("Jump"))
        {
            jumped = true;
            jumpSound.Play();

            //if the input is pointed towards the wall
            if (Mathf.Abs(wallDirX - moveX) < 0.5f)
            {
                outVelocity.x = -wallDirX * wallJumpClimbForces.x;
                outVelocity.y = wallJumpClimbForces.y;
            }
            //if the player has no x input
            else if (moveX == 0)
            {
                outVelocity.x = -wallDirX * wallJumpOffForces.x;
                outVelocity.y = wallJumpOffForces.y;
            }
            //assuming that the player is pointing away from the wall
            else
            {
                outVelocity.x = -wallDirX * wallLeapForces.x;
                outVelocity.y = wallLeapForces.y;
            }
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
