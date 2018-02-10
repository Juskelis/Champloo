using UnityEngine;
using System.Collections;

public class TauntState : MovementState {
    [SerializeField]
    private float tauntDuration;
    private float tauntTimer;

    public override bool AttackAllowed { get { return false; } }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        return Vector3.zero;
    }

    public override Vector3 DecayExternalForces(Vector3 externalForces)
    {
        return Vector3.zero;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        tauntTimer -= Time.deltaTime;
        if (tauntTimer <= 0)
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
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        tauntTimer = tauntDuration;
    }
}
