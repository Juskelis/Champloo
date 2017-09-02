using UnityEngine;
using System.Collections;


public class InBigBodyAttack : InAttack
{
    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        return GetSimulatedState().ApplyFriction(velocity);
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        movementSpecial.OnExitAttack(inVelocity, inExternalForces);
        outVelocity = inVelocity;
        outExternalForces = inExternalForces;
    }
}