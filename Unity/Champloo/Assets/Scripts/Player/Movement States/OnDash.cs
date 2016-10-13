using UnityEngine;
using System.Collections;

public class OnDash : OnMovementSpecial
{
    [SerializeField]
    private float dashForce;
    public float DashForce { get { return dashForce; } }

    [SerializeField]
    private float dashTime;
    private float timeLeft;

    [SerializeField]
    private float gravityModifier = 1f;

    [SerializeField]
    private int dashLimit;
    public int DashLimit { get { return dashLimit; } }

    private int currentDashes;

    private Vector2 direction;
    private Vector3 dashVelocity;

    TrailRenderer tail;

    protected override void Start()
    {
        base.Start();
        currentDashes = dashLimit;
        tail = GetComponent<TrailRenderer>();
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        //velocity.x += dashForce * direction.x * dashForce * Time.deltaTime;
        //velocity.y += dashForce * direction.y * dashForce * Time.deltaTime;

        if (!isDisabled)
        {
            velocity.y -= player.Gravity * Time.deltaTime * gravityModifier;

            //check for collisions in the x direction that the player is dashing
            if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
        {
            velocity.x = 0;
        }
        DecayExternalForces(ref externalForces);
        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);
        }
        else
        {
            timeLeft = -1;
        }

        if (timeLeft < 0)
        {

            //this probably needs to be changed so that it doesn't trigger on enemy players being below or to the side
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
        timeLeft -= Time.deltaTime;
        return null;
    }

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
        Debug.Log("Velocity on Enter:" + velocity);

        timeLeft = dashTime;

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

            Debug.Log("leftStickDir: " + leftStickDir);

            velocity = ((leftStickDir == Vector2.zero) ? Vector2.up : leftStickDir) * DashForce;

            Debug.Log("Velocity at End: " + velocity);

        }

        //If player does not have dashes
        else
        {
            isDisabled = true;
        }
    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
        tail.enabled = false;

    }



    //Functions to be called on state changes

    //ground state changes
    public override void OnEnterGround(ref Vector3 velocity, ref Vector3 externalForces)
    {
        currentDashes = DashLimit;
        isDisabled = false;
    }
    //public virtual void OnExitGround() { }

    //air state changes
    //public virtual void OnEnterAir() { }
    //public virtual void OnExitAir() { }

    //wallride state changes
    public override void OnEnterWall(ref Vector3 velocity, ref Vector3 externalForces)
    {
        if (currentDashes < 1)
    {
            currentDashes = 1;
        }
        isDisabled = false;
    }
    //public virtual void OnExitWall() { }
}
