using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBomb : OnMovementSpecial
{
    [SerializeField]
    private BombProjectile bombProjectile;

    private BombProjectile droppedBomb;

    private bool isThrown;

    protected override void OnStart()
    {
        base.OnStart();
        droppedBomb = Instantiate(
            bombProjectile,
            transform.position,
            Quaternion.AngleAxis(
                Utility.Vector2AsAngle(Vector2.down),
                transform.forward
            )
        );
    }

    public override MovementState DecideNextState(Vector3 velocity, Vector3 externalForces)
    {
        if (timingState == TimingState.DONE)
        {
            return GetSimulatedState();
        }
        return null;
    }
}
