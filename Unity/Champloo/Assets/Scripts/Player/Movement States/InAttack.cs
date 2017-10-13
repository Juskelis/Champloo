using UnityEngine;
using System.Collections;

public class InAttack : MovementState
{

    protected Weapon playerWeapon;
    protected Vector3 initialVelocity;

    protected override void Start()
    {
        base.Start();
        playerWeapon = GetComponentInChildren<Weapon>();
    }

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
        if (!playerWeapon.IsAttacking && !playerWeapon.IsSpecialAttacking)
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
        initialVelocity = inVelocity;
        movementSpecial.OnEnterAttack(inVelocity, inExternalForces);
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitAttack(inVelocity, inExternalForces);
        outVelocity = initialVelocity; 
    }
}
