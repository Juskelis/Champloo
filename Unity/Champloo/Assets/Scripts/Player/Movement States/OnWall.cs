using UnityEngine;
using System.Collections;

public class OnWall : MovementState
{
    [SerializeField]
    private float maxFallSpeed = 10f;

    [SerializeField]
    private float wallStickTime;

    [SerializeField]
    private Vector2 wallJumpVelocity;

    [SerializeField]
    private Vector2 noAngleJumpModifier;
    [SerializeField]
    private Vector2 towardsWallJumpModifier;
    //[SerializeField]
    //private Vector2 slightAngleJumpModifier;

    [SerializeField]
    private float allowedMinAngle = 1.16f;

    private float timeToWallUnstick;

    private bool jumped = false;


    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        if (jumped)
        {
            return velocity;
        }
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
        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }
        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        //normalize the direction so that it just gives the wall jump normalized vector
        Vector2 direction = player.AimDirection.normalized;
        int wallDirX = (controller.collisions.Left) ? -1 : 1;

        jumped = false;
        if (player.InputPlayer.GetButtonDown("Jump") && !inputController.IsConsumed("Jump"))
        {
            jumped = true;
            inputController.ConsumeButton("Jump");

            //PLayer will always jump off of a wall a little bit
            outVelocity.x = (wallJumpVelocity.x / 2) * (-wallDirX);

            //if player has no input
            //Player defaults no input to direction player is facing
            if (Mathf.Abs(direction.x) == 1 && direction.y == 0)
            {
                outVelocity.x = -wallDirX * wallJumpVelocity.x * noAngleJumpModifier.x;
                outVelocity.y = wallJumpVelocity.y;
            }
            //if the input is pointed towards the wall or slightly away and up
            else if (Mathf.Abs(wallDirX - direction.x) < 0.5f || ((Mathf.Abs(wallDirX - direction.x) < allowedMinAngle) && (Mathf.Abs(1 - direction.y) < .25)))
            {
                outVelocity.y = wallJumpVelocity.y;
            }
            //if the input is down and slightly away
            else if (Mathf.Abs(wallDirX - direction.x) < allowedMinAngle && Mathf.Abs(1 - direction.y) > .25)
            {
                outVelocity.y = -wallJumpVelocity.y;
            }
            //If player has other directional input
            else
            {
                outVelocity.x = wallJumpVelocity.x * direction.x;
                outVelocity.y = wallJumpVelocity.y * direction.y;
            }

            player.FireEvent(new JumpEvent {Active = this, Direction = outVelocity});
        }

        if (jumped)
        {
            player.UpdateDirection(wallDirX < 0);
        }
        else
        {
            player.UpdateDirection(wallDirX > 0);
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
    }
}
