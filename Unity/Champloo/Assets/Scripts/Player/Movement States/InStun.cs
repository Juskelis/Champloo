using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InStun : MovementState
{
    [SerializeField]
    private float friction;

    public override bool AttackAllowed { get { return false; } }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0, friction * Time.deltaTime);
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
}
