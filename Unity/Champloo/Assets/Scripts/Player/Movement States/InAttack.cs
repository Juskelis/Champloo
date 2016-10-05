using UnityEngine;
using System.Collections;

public class InAttack : MovementState
{
    Weapon playerWeapon;

    protected override void Start()
    {
        base.Start();
        playerWeapon = GetComponentInChildren<Weapon>();
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        velocity = Vector3.zero;
        externalForces = Vector3.zero;

        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);

        if (!playerWeapon.IsAttacking)
        {
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
        return null;
    }

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
        movementSpecial.OnEnterAttack(ref velocity, ref externalForces);
    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
        movementSpecial.OnExitAttack(ref velocity, ref externalForces);
    }
}
