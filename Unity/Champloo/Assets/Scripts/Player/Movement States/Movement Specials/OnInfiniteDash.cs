using UnityEngine;
using System.Collections;

public class OnInfiniteDash : OnDash
{
    private bool hitDashEarly;
    private bool attackAllowed;
    [SerializeField]
    private float attackBufferWindow;

    protected override void Start()
    {
        base.Start();
        hitDashEarly = false;
        attackAllowed = false;
    }

    public override bool AttackAllowed { get { return attackAllowed; } }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;
        //Buffer for attacks and movement specials done too early

        attackAllowed = specialTimeLeft < (specialTime * attackBufferWindow);

        if (specialTimeLeft < (specialTime * nextDashBufferWindow))
        {
            if (player.InputPlayer.GetButtonDown("Movement Special") && !hitDashEarly)
            {
                hitNextDashInput = true;
                currentDashes++;
            }
        }
        else if (player.InputPlayer.GetButtonDown("Movement Special"))
        {
            hitDashEarly = true;
        }

        if (isDisabled || timingState == TimingState.DONE)
        {
            hitDashEarly = false;
            if (hitNextDashInput)
            {
                hitNextDashInput = false;
                return GetComponent<OnInfiniteDash>();
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

    //remove functionality of OnDash's OnEnterWall
    public override void OnEnterWall(Vector3 velocity, Vector3 externalForces)
    {
    }
    

    public override void OnExit(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        base.OnExit(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
        attackAllowed = false;
    }
}