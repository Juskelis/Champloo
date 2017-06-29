using UnityEngine;
using System.Collections;

public class OnGround : MovementState
{
    [SerializeField]
    private float maxSpeed = 6;
    [SerializeField]
    private float stopToMaxSpeedTime = 0.1f;
    [SerializeField]
    private float maxSpeedToStopTime = 0.2f;
    [SerializeField]
    private float fullTurnTime = 0.2f;

    [SerializeField]
    private PlayRandomSource jumpSound;

    [SerializeField]
    private PlayRandomSource runSound;

    [SerializeField]
    private bool analogMovementSpeed = false;

    public float MaxSpeed { get { return maxSpeed; } }

    private float jumpVelocity;

    private float acceleration;
    private float deceleration;
    private float turningDeceleration;

    public bool Jumped { get; set; }

    protected override void Start()
    {
        base.Start();
        //maxJumpVelocity = player.Gravity * player.timeToJumpApex;

        jumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(player.Gravity) * player.maxJumpHeight);

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
        return velocity;
    }

    public override void ApplyInputs(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.ApplyInputs(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        Jumped = false;
        if (player.InputPlayer.GetButtonDown("Jump"))
        {
            Jumped = true;
            jumpSound.Play();
            player.FireEvent(new JumpEvent {Active = this, Direction = Vector3.up});
            outVelocity.y = jumpVelocity;
        }

        float moveX = player.InputPlayer.GetAxis("Move Horizontal");
        float inputDirection = Mathf.Sign(moveX);
        if (Mathf.Abs(moveX) >= float.Epsilon)
        {
            float delta = inputDirection != Mathf.Sign(inVelocity.x) ? turningDeceleration : acceleration;
            outVelocity.x = Mathf.MoveTowards(
                inVelocity.x,
                maxSpeed*(analogMovementSpeed?moveX:inputDirection),
                delta*Time.deltaTime);
        }

        if (Mathf.Abs(outVelocity.x) >= float.Epsilon && !runSound.Playing)
        {
            runSound.Play();
        }
        else if (Mathf.Abs(outVelocity.x) < float.Epsilon)
        {
            runSound.Stop();
        }
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (!controller.collisions.Below || Jumped)
        {

            if (controller.collisions.Left || controller.collisions.Right)
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
        movementSpecial.OnEnterGround(inVelocity, inExternalForces);
        outVelocity.y = 0;
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        movementSpecial.OnExitGround(inVelocity, inExternalForces);
        runSound.Stop();
        Jumped = false;
    }

}
