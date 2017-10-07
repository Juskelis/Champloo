using UnityEngine;
using System.Collections;


public class InBigBodyAttack : InAttack
{
    [SerializeField]
    private float groundFriction;
    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (GetSimulatedState() is OnGround)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, groundFriction * Time.deltaTime);
            return velocity;
        }
        else
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }
    }
    
    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        movementSpecial.OnExitAttack(inVelocity, inExternalForces);
        outVelocity = inVelocity;
        outExternalForces = inExternalForces;
    }
}