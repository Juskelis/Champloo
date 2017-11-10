using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDarkbladeAttack : InAttack
{
    [SerializeField]
    [Tooltip("Time it takes from InAir's max speed to stopped, tied to Weapon's special startup time")]
    [Range(0,1)]
    private float maxStopTime;

    [SerializeField]
    private float exitSpeedMultiplier = 1f;

    private InAir inAirState;
    private float inAirMaxSpeed;

    private float specialFriction;

    private bool isSpecial = false;

    protected override void Start()
    {
        base.Start();
        inAirState = GetComponent<InAir>();
        inAirMaxSpeed = inAirState.MaxFallSpeed;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (playerWeapon.IsSpecialAttacking && playerWeapon.SpecialAttackState == TimingState.WARMUP)
        {
            return Vector3.MoveTowards(velocity, Vector3.zero, specialFriction*Time.deltaTime);
        }

        if (playerWeapon.IsSpecialAttacking)
        {
            isSpecial = true;
        }

        return base.ApplyFriction(velocity);
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        if (isSpecial)
        {
            outVelocity = outVelocity*exitSpeedMultiplier;
        }
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        isSpecial = false;
        specialFriction = inVelocity.sqrMagnitude > inAirMaxSpeed*inAirMaxSpeed
            ? inVelocity.magnitude/(maxStopTime*playerWeapon.SpecialStartupTime)
            : inAirMaxSpeed / (maxStopTime * playerWeapon.SpecialStartupTime);
    }
}
