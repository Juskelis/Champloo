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

    public float MaxSpeed { get { return maxSpeed; } }

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

        if ((controller.collisions.Above && velocity.y > 0) || (controller.collisions.Below && velocity.y < 0))
        {
            velocity.y = 0;
        }

        Jumped = false;
        //if (input.jump.Down)
        if(player.InputPlayer.GetButtonDown("Jump"))
        {
            Jumped = true;
            //input.jump.ResetTimers();
            velocity.y = maxJumpVelocity;
        }

        DecayExternalForces(ref externalForces);

        controller.Move(velocity*Time.deltaTime + externalForces*Time.deltaTime);

        if (!controller.collisions.Below || Jumped)
        {
            
            if (controller.collisions.Left || controller.collisions.Right)
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
