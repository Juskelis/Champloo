using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InStun : MovementState
{
    [SerializeField]
    private float friction;

    public override bool AttackAllowed { get { return false; } }

    private InAir inAirState;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0, friction * Time.deltaTime);

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -inAirState.MaxFallSpeed) velocity.y = -inAirState.MaxFallSpeed;

        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (!player.Stunned)
        {
            return GetSimulatedState();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        inAirState = GetComponent<InAir>();
    }
}
