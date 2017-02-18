using UnityEngine;
using System.Collections;
using Rewired;

public class InBlock : MovementState
{
    [SerializeField]
    private float maxSpeedToStopTime = 0.1f;

    private float deceleration;

    private float maxFallSpeed;

    private Shield ourShield;

    protected override void Start()
    {
        base.Start();

        ourShield = GetComponentInChildren<Shield>();
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * Time.deltaTime);

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        return velocity;
    }

    public override Vector3 DecayExternalForces(Vector3 externalForces)
    {
        return Vector3.zero;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if(!player.InputPlayer.GetButton("Block") || !ourShield.Up)
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

    public override void OnEnter(
        Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        movementSpecial.OnEnterBlock(inVelocity, inExternalForces);
        deceleration = GetComponent<OnGround>().MaxSpeed/maxSpeedToStopTime;
        maxFallSpeed = GetComponent<InAir>().MaxFallSpeed;
        ourShield.ActivateShield();
    }

    public override void OnExit(
        Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        movementSpecial.OnExitBlock(inVelocity, inExternalForces);
        ourShield.DeactivateShield();
    }
}
