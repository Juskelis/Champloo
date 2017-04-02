using UnityEngine;
using System.Collections;

public class OnTether : OnMovementSpecial
{
    [SerializeField]
    private float tetherForce;
    public float TetherForce { get { return tetherForce; } }

    [SerializeField]
    private float tetherTime;
    private float timeLeft;

    [SerializeField]
    private float gravityModifier = 1f;

    private Weapon weapon;
    private Projectile temp;

    private Vector2 direction;
    private Vector3 tetherVelocity;

    private bool isThrown;

    private Collider2D col;
    private MovementState currentState;

    TrailRenderer tail;

    protected override void Start()
    {
        base.Start();
        tail = GetComponent<TrailRenderer>();
        weapon = GetComponentInChildren<Weapon>();
        col = GetComponent<Collider2D>();
    }

    //overridden so we can manually trigger OnEnd()
    protected override IEnumerator TimingCoroutine()
    {
        timingState = TimingState.WARMUP;
        yield return new WaitForSeconds(startupTime);
        OnStart();
        while (timingState == TimingState.IN_PROGRESS)
        {
            yield return null;
        }
        //we don't end up calling OnEnd here b/c
        //  we want user (other methods in this class)
        //  to call it to transition between states
        yield return new WaitForSeconds(cooldownTime);
        OnCooledDown();
    }

    public override Vector3 ApplyFriction(Vector3 velocity)
    {
        if (timingState != TimingState.IN_PROGRESS)
        {
            return GetSimulatedState().ApplyFriction(velocity);
        }
        if (!isThrown || !temp)
        {
            OnEnd();
            return GetSimulatedState().ApplyFriction(velocity);
        }

        //find the direction between the player and the stopped weapon
        //then pull the player continuously towards the tether
        Vector2 pullDirection = (temp.transform.position - player.transform.position).normalized;
        velocity = pullDirection*tetherForce;

        if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
        {
            velocity.x = 0;
        }
        if ((controller.collisions.Above && direction.y > 0) || (controller.collisions.Below && direction.x < 0))
        {
            velocity.y = 0;
        }

        return velocity;
    }

    public override Vector3 DecayExternalForces(Vector3 externalForces)
    {
        if (timingState != TimingState.IN_PROGRESS) return externalForces;
        return base.DecayExternalForces(externalForces);
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (timingState == TimingState.WARMUP)
        {
            return null;
        }
        if (timingState == TimingState.DONE)
        {
            if (controller.collisions.Below)
            {
                return GetComponent<OnGround>();
            }

            if (controller.collisions.Left || controller.collisions.Right)
            {
                return GetComponent<OnWall>();
            }

            return GetComponent<InAir>();
        }
        return null;
    }

    protected override void OnStart()
    {
        base.OnStart();
        if(!weapon.InHand)
        {
            isThrown = false;
        }
        else if(!isDisabled)
        {
            isDisabled = true;
            weapon.InHand = false;
            isThrown = true;

            Vector2 aim = player.AimDirection;
            temp = (Projectile)Instantiate(
                weapon.ThrownVersion,
                weapon.transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(aim),
                    weapon.transform.parent.forward
                )
            );

            temp.PlayerNumber = player.PlayerNumber;
        }
    }

    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);

        //set up the tether visual trail
        tail.enabled = true;
        tail.Clear();

    }

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        tail.enabled = false;
        isThrown = false;
        isDisabled = false;
    }
}