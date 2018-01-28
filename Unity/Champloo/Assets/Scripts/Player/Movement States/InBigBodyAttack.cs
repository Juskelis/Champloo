using UnityEngine;
using System.Collections;


public class InBigBodyAttack : InAttack
{
    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private float lerpSpeed = 0.6f;

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (GetSimulatedState() is OnGround)
        {
            float targetSpeed = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            velocity.x = Mathf.Pow(lerpSpeed, 3)*(targetSpeed - velocity.x)/2 + velocity.x;
        }
        return GetSimulatedState().ApplyFriction(velocity);
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        outVelocity = inVelocity;
    }
}