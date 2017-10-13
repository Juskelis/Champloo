using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDarkbladeAttack : InAttack
{
    [SerializeField]
    [Tooltip("Time it takes from InAir's max speed to stopped")]
    private float maxStopTime;

    private InAir inAirState;
    private float inAirMaxSpeed;

    private float specialFriction;
    private float inAirSpecialFriction;

    protected override void Start()
    {
        base.Start();
        inAirState = GetComponent<InAir>();
        inAirMaxSpeed = inAirState.MaxFallSpeed;
        specialFriction = inAirMaxSpeed/maxStopTime;
        inAirSpecialFriction = specialFriction;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (playerWeapon.IsSpecialAttacking && playerWeapon.SpecialAttackState == TimingState.WARMUP)
        {
            return Vector3.MoveTowards(velocity, Vector3.zero, specialFriction*Time.deltaTime);
        }
        return base.ApplyFriction(velocity);
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        specialFriction = inVelocity.sqrMagnitude > inAirMaxSpeed*inAirMaxSpeed
            ? inVelocity.magnitude/maxStopTime
            : inAirSpecialFriction;
    }
}
