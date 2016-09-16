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
    private float timeToWallUnstick;

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        int wallDirX = (controller.collisions.Left) ? -1 : 1;
        //velocity.x = input.leftStick.x;
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        velocity.x = moveX;
        if (timeToWallUnstick > 0)
        {
            velocity.x = 0;

            if (moveX != 0 && moveX != wallDirX)
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
        //if (input.jump.Down)
        if (player.InputPlayer.GetButtonDown("Jump"))
        {
            jumped = true;
            //input.jump.ResetTimers();

            if (Mathf.Abs(wallDirX - moveX) < 0.5f)
            {
                velocity.x = -wallDirX * wallJumpClimbForces.x;
                velocity.y = wallJumpClimbForces.y;
            }
            else if (moveX == 0)
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

        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }
        else if (jumped || !(controller.collisions.Left || controller.collisions.Right))
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
