using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InStun : MovementState
{

    private MovementState getSimulatedState()
    {
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }
        else if (controller.collisions.Left || controller.collisions.Right)
        {
            return GetComponent<OnWall>();
        }
        return GetComponent<InAir>();
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        return getSimulatedState().ApplyFriction(velocity);
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (!player.Stunned)
        {
            return getSimulatedState();
        }
        return null;
    }
}
