using System;
using UnityEngine;
using System.Collections;

public class OnDash : OnMovementSpecial
{
    [SerializeField]
    protected float dashForce;
    public float DashForce { get { return dashForce; } }

    //private float timeLeft;

    [SerializeField]
    protected float gravityModifier;

    [SerializeField]
    protected int dashLimit;
    public int DashLimit { get { return dashLimit; } }

    [SerializeField]
    protected float nextDashBufferWindow;
    
    
    protected int currentDashes;
    public int DashesRemaining { get { return currentDashes; } }

    private Vector2 direction;
    public Vector2 Direction { get { return direction; } }
    private Vector3 dashVelocity;

    [SerializeField]
    protected float attackBufferWindow;

    protected bool earlyAttackInput;
    protected bool hitNextDashInput;
    private bool justStarted;

    protected bool onStartCalled;

    private bool childAttackAllowed = true;
    public override bool AttackAllowed
    {
        get { return childAttackAllowed; }
        protected set { childAttackAllowed = value; }
    }

    protected override void Start()
    {
        base.Start();
        currentDashes = dashLimit;
        AttackAllowed = false;
        hitNextDashInput = false;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (isDisabled || timingState != TimingState.IN_PROGRESS)
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }
        if (justStarted)
        {
            justStarted = false;
            return direction;
        }
        velocity.y -= player.Gravity * Time.deltaTime * gravityModifier;

        //check for collisions in the x direction that the player is dashing
        if ((controller.collisions.Left && velocity.x < 0) || (controller.collisions.Right && velocity.x > 0))
        {
            velocity.x = 0;
        }

        if (controller.collisions.Above && velocity.y > 0)
        {
            velocity.y = 0;
        }

        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;

        //Buffer for attacks and movement specials done too early
        if (specialTimeLeft < (specialTime * nextDashBufferWindow))
        {
            if (player.InputPlayer.GetButtonDown("Movement Special"))
            {
                hitNextDashInput = true;
            }
        }

        AttackAllowed = (attackBufferWindow > 0 && specialTimeLeft < (specialTime * attackBufferWindow) && onStartCalled);

        if (AttackAllowed && player.Weapon.IsAttacking)
        {
            return GetComponent<InAttack>();
        }
        if (isDisabled || timingState == TimingState.RECHARGE || timingState == TimingState.DONE)
        {
            if(hitNextDashInput)
            {
                return GetComponent<OnDash>();
            }
            else if (controller.collisions.Below)
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
        //short out
        if (currentDashes <= 0)
        {
            timingState = TimingState.DONE;

            outVelocity = inVelocity;
            outExternalForces = inExternalForces;

            return;
        }

        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);


        //Establish the direction of the dash
        direction = player.AimDirection.normalized;

        currentDashes--;
        isDisabled = false;

		//actually apply dash forces
		//take player directional input, apply dash force in that direction, clamping on max speed


		//Potential Code if we want to replicate the auto-dash-angles in the demo when you jump and dash 
		//Vector3 dashVector = direction * DashForce;
		//if (dashVector.y != 0)
		//{
		//    //if the dash is in the opposite direction the player is currently going
		//    if (Mathf.Sign(dashVector.y) != Mathf.Sign(inVelocity.y))
		//    {
		//        inVelocity.y = 0;
		//    }
		//}
		//if (dashVector.x != 0)
		//{
		//    //if the dash is in the opposite direction the player is currently going
		//    if (Mathf.Sign(dashVector.x) != Mathf.Sign(inVelocity.x))
		//    {
		//        inVelocity.x = 0;
		//    }
		//}
		//direction = inVelocity + dashVector;
		
        direction = direction * DashForce;
        justStarted = false;
    }

    protected override void OnStart()
    {
        base.OnStart();
        justStarted = true;
        onStartCalled = true;
        specialTimeLeft = specialTime;
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        AttackAllowed = false;
        hitNextDashInput = false;
        onStartCalled = false;
    }

    protected override void OnStateChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        if (e.Next is OnGround)
        {
            currentDashes = DashLimit;
        } else if (currentDashes <= 1 && e.Next is OnWall)
        {
            currentDashes = 1;
        }
    }
}
