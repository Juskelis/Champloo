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

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;

    protected override void Start()
    {
        base.Start();

        //vT = v0 + at
        acceleration = maxSpeed / stopToMaxSpeedTime;
        deceleration = maxSpeed / maxSpeedToStopTime;
        turningDeceleration = (maxSpeed * 2) / fullTurnTime;
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        //float inputDirection = Mathf.Sign(input.leftStick.x);
        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        float inputDirection = Mathf.Sign(moveX);
        if (Mathf.Abs(moveX) > float.Epsilon)
        {
            if (inputDirection != Mathf.Sign(velocity.x))
            {
                //turning
                velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * moveX, turningDeceleration * Time.deltaTime);
            }
            else
            {
                //speeding up
                velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * moveX, acceleration * Time.deltaTime);
            }
        }
        else
        {
            //stopping
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity*Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;
        

        DecayExternalForces(ref externalForces);

        controller.Move(velocity*Time.deltaTime + externalForces*Time.deltaTime);


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

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
        movementSpecial.OnEnterAir(ref velocity, ref externalForces);
    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
        movementSpecial.OnExitAir(ref velocity, ref externalForces);
    }
}
