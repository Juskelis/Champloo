using UnityEngine;
using System.Collections;

public class InBlock : MovementState
{
    [SerializeField]
    private float maxSpeedToStopTime = 0.1f;

    private float deceleration;

    private float maxFallSpeed;

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * Time.deltaTime);

        if (controller.collisions.above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        externalForces = Vector3.zero;

        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);

        if(!input.block.Pressed)
        {
            if (controller.collisions.below)
            {
                return GetComponent<OnGround>();
            }
            else if (controller.collisions.left || controller.collisions.right)
            {
                return GetComponent<OnWall>();
            }
            return GetComponent<InAir>();
        }
        return null;
    }

    public override void OnEnter()
    {
        deceleration = GetComponent<OnGround>().MaxSpeed/maxSpeedToStopTime;
        maxFallSpeed = GetComponent<InAir>().MaxFallSpeed;
    }
}
