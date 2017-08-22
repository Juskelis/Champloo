using UnityEngine;
using System.Collections;
public class OnKillerDash : OnDash
{

    public void Start()
    {
        base.Start();
        timingState = TimingState.DONE;
    }
    public override void OnEnter(Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        //short out
        if (timingState == TimingState.RECHARGE)
        {
            outVelocity = inVelocity;
            outExternalForces = inExternalForces;
            return;
        }
        base.OnEnter(inVelocity, inExternalForces, out outVelocity, out outExternalForces);
    }

    protected override void OnStart()
    {
        base.OnStart();
        currentDashes = 0;
    }

    protected override void OnCooledDown()
    {
        currentDashes = 1;
        base.OnCooledDown();
    }
}
