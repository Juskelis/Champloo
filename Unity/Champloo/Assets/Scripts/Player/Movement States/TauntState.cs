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
            if (controller.collisions.below)
            {
                return GetComponent<OnGround>();
            }
            else if (controller.collisions.left || controller.collisions.right)
            {
                return GetComponent<OnWall>();
            }
            return GetComponent<InAir>();
        }
        tauntTimer -= Time.deltaTime;
        return null;
    }

    public override void OnEnter()
    {
        tauntTimer = tauntDuration;
    }

    public override void OnExit()
    {
    }
}
