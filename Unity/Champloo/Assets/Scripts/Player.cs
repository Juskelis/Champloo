using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{

    private InputController inputs;
    
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float timeToJumpApex = 0.4f;
    [SerializeField] private float accelerationTimeInAir = 0.2f;
    [SerializeField] private float accelerationTimeOnGround = 0.1f;
    [SerializeField] private Vector2 wallJumpClimb;
    [SerializeField] private Vector2 wallJumpOff;
    [SerializeField] private Vector2 wallLeap;
    [SerializeField] private float wallStickTime = 0.25f;
    private float timeToWallUnstick;

    private float moveSpeed = 6;

    [SerializeField] private float wallSlideSpeedMax = 3;

    private Controller2D controller;

    private Vector3 velocity = Vector3.zero;
    
    public float Gravity { get; set; }

    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float velocityXSmoothing;
    
    private MovementState movementState;

    void Start ()
    {
        movementState = GetComponent<OnGround>();

	    controller = GetComponent<Controller2D>();

        inputs = GetComponent<InputController>();
        inputs.playerNumber = 1;

        //derived from: deltaMovement = velocityInitial*time + (accleration*time^2)/2
        //gravity = -1*(2*maxJumpHeight)/(timeToJumpApex*timeToJumpApex);

        //derived from: velocityFinal = velocityInitial + acceleration*time
        //maxJumpVelocity = -1*gravity*timeToJumpApex;

        //minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity)*minJumpHeight);
	}

    void Update()
    {
        inputs.UpdateInputs();
        
        MovementState next = movementState.UpdateState(ref velocity);

        if (inputs.movementSpecial.Down)
        {
            next = GetComponent<OnDash>();
            velocity = inputs.leftStick*((OnDash) next).DashForce;
        }

        if (next != null)
        {
            movementState.OnExit();
            next.OnEnter();
            movementState = next;
        }
        
        /*
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            controller.collisions.below ? accelerationTimeOnGround : accelerationTimeInAir);

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != 0 && input.x != wallDirX)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (jump.Down)
        {
            jump.ResetTimers();
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    velocity.x = -wallDirX*wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX*wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }
                else
                {
                    velocity.x = -wallDirX*wallLeap.x;
                    velocity.y = wallLeap.y;
                }
            }
            else if (controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
            }
        }
        if (jump.Up && velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }

        velocity.y += gravity*Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);
        */
    }
}
