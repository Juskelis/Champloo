using UnityEngine;
using System.Collections;

public class OnInfiniteDash : OnDash
{
    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        specialTimeLeft -= Time.deltaTime;

        //Buffer for attacks and movement specials done too early
        if (specialTimeLeft < (specialTime * nextDashBufferWindow))
        {
            if (player.InputPlayer.GetButtonDown("Attack"))
            {
                earlyAttackInput = true;
            }
            else if (player.InputPlayer.GetButtonDown("Movement Special"))
            {
                hitNextDashInput = true;
                currentDashes++;
            }
        }

        if (isDisabled || timingState == TimingState.DONE)
        {
            if (earlyAttackInput)
            {
                return GetComponent<InAttack>();
            }
            else if (hitNextDashInput)
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
}