using UnityEngine;
using System.Collections;

public class TauntState : MovementState {
    [SerializeField]
    private float tauntDuration;
    private float tauntTimer;

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        velocity = Vector3.zero;
        externalForces = Vector3.zero;

        controller.Move(velocity * Time.deltaTime + externalForces*Time.deltaTime);

        if (tauntTimer <= 0)
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
        tauntTimer -= Time.deltaTime;
        return null;
    }

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
        tauntTimer = tauntDuration;
        movementSpecial.OnEnterTaunt(ref velocity, ref externalForces);
    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
        movementSpecial.OnExitTaunt(ref velocity, ref externalForces);
    }
}
