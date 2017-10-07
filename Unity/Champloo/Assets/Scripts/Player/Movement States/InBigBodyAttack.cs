using UnityEngine;
using System.Collections;


public class InBigBodyAttack : InAttack
{
    [SerializeField]
    private float groundFriction;
    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        return GetSimulatedState().ApplyFriction(velocity);
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        if (GetSimulatedState() is OnGround)
        {
            outVelocity.x = Mathf.MoveTowards(outVelocity.x, 0, groundFriction * Time.deltaTime);
        }
    }
    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        movementSpecial.OnExitAttack(inVelocity, inExternalForces);
        outVelocity = inVelocity;
        if(GetSimulatedState() is OnGround)
        {
            outVelocity.x = Mathf.MoveTowards(outVelocity.x, 0, groundFriction * Time.deltaTime);
        }
        outExternalForces = inExternalForces;
    }
}