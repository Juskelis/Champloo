using UnityEngine;
using System.Collections;

public class OnWall : MovementState
{
    [SerializeField]
    private float jumpWindow = 0.1f;

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
    
    public bool Jumped { get; private set; }

    private int previousWallDirX;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        if (Jumped)
        {
            return velocity;
        }
        if (timeToWallUnstick > 0)
        {
            if (wallDirX < 0 && velocity.x < 0)
            {
                velocity.x = 0;
            }
            if (wallDirX > 0 && velocity.x > 0)
            {
                velocity.x = 0;
            }

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

    public Vector3 OnJump()
    {
        Vector2 direction = player.AimDirection.normalized;

        int wallDirX = previousWallDirX;
        if (controller.collisions.Left)
        {
            wallDirX = -1;
        } else if (controller.collisions.Right)
        {
            wallDirX = 1;
        }

        Vector3 outVelocity = Vector3.zero;
        Jumped = true;
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
            outVelocity.y = wallJumpVelocity.y * towardsWallJumpModifier.y;
            outVelocity.x *= towardsWallJumpModifier.x;
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

        player.FireEvent(new JumpEvent { Active = this, Direction = outVelocity });
        return outVelocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        //normalize the direction so that it just gives the wall jump normalized vector
        Vector2 direction = player.AimDirection.normalized;
        int wallDirX = (controller.collisions.Left) ? -1 : 1;

        Jumped = false;
        if (inputController.IsDown("Jump", jumpWindow))
        {
            outVelocity = OnJump();
        }

        if (Jumped)
        {
            player.UpdateDirection(wallDirX < 0);
        }
        else
        {
            player.UpdateDirection(wallDirX > 0);
        }

        previousWallDirX = wallDirX;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }

        if (Jumped || !(controller.collisions.Left || controller.collisions.Right))
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
        Jumped = false;
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitWall(inVelocity, inExternalForces);
    }
}
