using UnityEngine;
using System.Collections;

public class OnDash : OnMovementSpecial
{
    [SerializeField]
    private float dashForce;
    public float DashForce { get { return dashForce; } }
    
    //private float timeLeft;

    [SerializeField]
    private float gravityModifier = 1f;

    [SerializeField]
    private int dashLimit;
    public int DashLimit { get { return dashLimit; } }
    
    private int currentDashes;

    private Vector2 direction;
    private Vector3 dashVelocity;

    
    private bool earlyDashInput;
    TrailRenderer tail;

    protected override void Start()
    {
        base.Start();
        currentDashes = dashLimit;
        tail = GetComponent<TrailRenderer>();
        earlyDashInput = false;
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if(isDisabled) return velocity;

        velocity.y -= player.Gravity * Time.deltaTime * gravityModifier;

        //check for collisions in the x direction that the player is dashing
        if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
        {
            velocity.x = 0;
        }

        return velocity;
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;
        if (specialTimeLeft < 0 || isDisabled)
        {
            //this probably needs to be changed so that it doesn't trigger on enemy players being below or to the side
          
            if(earlyDashInput)
            {
                Debug.Log("IN EARLY DASH");
                return GetComponent<OnDash>();
            }
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
        //Buffer for attacks and movement specials done too early

        if (specialTimeLeft < (specialTime / 5))
        {
            if(player.InputPlayer.GetButtonDown("Movement Special"))
            {
                earlyDashInput = true;
            }
        }
        
        return null;
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        specialTimeLeft = specialTime;

        //set up the dash visual trail
        tail.enabled = true;
        tail.Clear();

        //Establish the direction of the dash
        Rewired.Player p = player.InputPlayer;
        direction = Vector2.right * p.GetAxis("Aim Horizontal") + Vector2.up * p.GetAxis("Aim Vertical");

        //Check if the player still has dashes
        if (currentDashes > 0)
        {
            currentDashes--;
            isDisabled = false;

            //actually apply dash forces
            Vector2 leftStickDir =
                (Vector2.right * player.InputPlayer.GetAxis("Aim Horizontal") +
                 Vector2.up * player.InputPlayer.GetAxis("Aim Vertical")).normalized;
            
            outVelocity = ((leftStickDir == Vector2.zero) ? Vector2.up : leftStickDir) * DashForce;
        }

        //If player does not have dashes
        else
        {
            isDisabled = true;
        }
    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        earlyDashInput = false;
        tail.enabled = false;
    }
    
    //Functions to be called on state changes

    //ground state changes
    public override void OnEnterGround(Vector3 velocity, Vector3 externalForces)
    {
        currentDashes = DashLimit;
        isDisabled = false;
    }

    //wallride state changes
    public override void OnEnterWall(Vector3 velocity, Vector3 externalForces)
    {
        if (currentDashes < 1)
        {
            currentDashes = 1;
        }
        isDisabled = false;
    }
}
