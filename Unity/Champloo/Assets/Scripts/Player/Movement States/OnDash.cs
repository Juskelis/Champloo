using UnityEngine;
using System.Collections;

public class OnDash : MovementState
{
    [SerializeField] private float dashForce;
    public float DashForce { get { return dashForce; } }

    [SerializeField] private float dashTime;
    private float timeLeft;

    [SerializeField] private float gravityModifier = 1f;

    private Vector2 direction;

    public override MovementState UpdateState(ref Vector3 velocity)
    {
        //velocity.x += dashForce * direction.x * dashForce * Time.deltaTime;
        //velocity.y += dashForce * direction.y * dashForce * Time.deltaTime;

        velocity.y -= player.Gravity*Time.deltaTime*gravityModifier;

        if(controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
        }

        controller.Move(velocity * Time.deltaTime);

        if (timeLeft < 0)
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
        timeLeft -= Time.deltaTime;
        return null;
    }

    public override void OnEnter()
    {
        timeLeft = dashTime;
        direction = input.leftStick;
    }

    public override void OnExit()
    {
    }
}
