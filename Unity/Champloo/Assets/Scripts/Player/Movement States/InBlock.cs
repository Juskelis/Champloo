using UnityEngine;
using System.Collections;

public class InBlock : MovementState {

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime);
        externalForces = Vector3.zero;

        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);

        if(!input.block.Pressed)
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
        return null;
    }

}
