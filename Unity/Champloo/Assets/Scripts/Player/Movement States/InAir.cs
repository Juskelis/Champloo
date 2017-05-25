using UnityEngine;
using System.Collections;
using Rewired;

public class InAir : MovementState
{
    [SerializeField]
    private float maxFallSpeed = 20f;
    public float MaxFallSpeed { get { return maxFallSpeed; } }

    [SerializeField]
    private float maxSpeed = 6;
    [SerializeField]
    private float stopToMaxSpeedTime = 0.1f;
    [SerializeField]
    private float maxSpeedToStopTime = 0.2f;
    [SerializeField]
    private float fullTurnTime = 0.2f;

    [SerializeField]
    private bool analogMovementSpeed = false;

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;
    private bool hasShortened;

    protected override void Start()
    {
        base.Start();

        //vT = v0 + at
        acceleration = maxSpeed / stopToMaxSpeedTime;
        deceleration = maxSpeed / maxSpeedToStopTime;
        turningDeceleration = (maxSpeed * 2) / fullTurnTime;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        if (Mathf.Abs(moveX) <= float.Epsilon)
        {
            //stopping
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces, out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        float inputDirection = Mathf.Sign(moveX);
        if (Mathf.Abs(moveX) > float.Epsilon)
        {
            float delta = inputDirection != Mathf.Sign(inVelocity.x) ? turningDeceleration : acceleration;
            outVelocity.x = Mathf.MoveTowards(
                inVelocity.x,
                maxSpeed*(analogMovementSpeed ? moveX : inputDirection),
                delta*Time.deltaTime);
        }

        //if the player releases the jump button and is is moving up
        //if (player.InputPlayer.GetButtonUp("Jump") && inVelocity.y > 0 && !hasShortened)
        if (player.InputPlayer.GetButtonUp("Jump") && !hasShortened)
        {
            outVelocity.y = inVelocity.y / 2;
            hasShortened = true;
        }
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }
        else if (controller.collisions.Left || controller.collisions.Right)
        {
            return GetComponent<OnWall>();
        }
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnEnterAir(inVelocity, inExternalForces);
        hasShortened = false;
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitAir(inVelocity, inExternalForces);
    }
}
