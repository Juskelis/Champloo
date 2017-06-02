using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InStun : MovementState
{
    [SerializeField]
    private float defaultStunDuration;

    private float stunDuration;
    private float stunTimer;
    private bool setStunDuration = false;

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

    public void SetStunTime(float time)
    {
        stunDuration = time;
        setStunDuration = true;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        return getSimulatedState().ApplyFriction(velocity);
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            return getSimulatedState();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        stunTimer = setStunDuration ? stunDuration : defaultStunDuration;
        print("stunned");
    }
}
