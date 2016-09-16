using UnityEngine;
using System.Collections;
using Rewired;

public class InBlock : MovementState
{
    [SerializeField]
    private float maxSpeedToStopTime = 0.1f;

    private float deceleration;

    private float maxFallSpeed;

    private Shield ourShield;

    protected override void Start()
    {
        base.Start();

        ourShield = GetComponentInChildren<Shield>();
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, deceleration * Time.deltaTime);

        if (controller.collisions.Above && velocity.y > 0) velocity.y = 0;

        velocity.y -= player.Gravity * Time.deltaTime;

        if (velocity.y < -maxFallSpeed) velocity.y = -maxFallSpeed;

        externalForces = Vector3.zero;

        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);

        //if(!input.block.Pressed || !ourShield.Up)
        if(!player.InputPlayer.GetButton("Block") || !ourShield.Up)
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

    public override void OnEnter()
    {
        deceleration = GetComponent<OnGround>().MaxSpeed/maxSpeedToStopTime;
        maxFallSpeed = GetComponent<InAir>().MaxFallSpeed;
        ourShield.ActivateShield();
    }

    public override void OnExit()
    {
        ourShield.DeactivateShield();
    }
}
