using UnityEngine;
using System.Collections;

public class OnGround : MovementState
{
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float timeToJumpApex = 0.4f;
    [Space]
    [SerializeField] private float maxSpeed = 6;
    [SerializeField] private float stopToMaxSpeedTime = 0.1f;
    [SerializeField] private float maxSpeedToStopTime = 0.2f;
    [SerializeField] private float fullTurnTime = 0.2f;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;

    public bool Jumped { get; set; }

    protected override void Start()
    {
        base.Start();
        player.Gravity = (2 * maxJumpHeight) / (timeToJumpApex * timeToJumpApex);

        maxJumpVelocity = player.Gravity * timeToJumpApex;

        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(player.Gravity) * minJumpHeight);

        //vT = v0 + at
        acceleration = maxSpeed/stopToMaxSpeedTime;
        deceleration = maxSpeed/maxSpeedToStopTime;
        turningDeceleration = (maxSpeed*2)/fullTurnTime;
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        float inputDirection = Mathf.Sign(input.leftStick.x);
        if (Mathf.Abs(input.leftStick.x) > float.Epsilon)
        {
            if (inputDirection != Mathf.Sign(velocity.x))
            {
                //turning
                velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * input.leftStick.x, turningDeceleration * Time.deltaTime);
            }
            else
            {
                //speeding up
                velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed * input.leftStick.x, acceleration * Time.deltaTime);
            }
        }
        else
        {
            //stopping
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Jumped = false;
        if (input.jump.Down)
        {
            Jumped = true;
            input.jump.ResetTimers();
            velocity.y = maxJumpVelocity;
        }

        DecayExternalForces(ref externalForces);

        controller.Move(velocity*Time.deltaTime + externalForces*Time.deltaTime);

        if (!controller.collisions.below || Jumped)
        {
            
            if (controller.collisions.left || controller.collisions.right)
            {
                return GetComponent<OnWall>(); //wallriding
            }
            return GetComponent<InAir>(); //inair
        }
        return null;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

}
