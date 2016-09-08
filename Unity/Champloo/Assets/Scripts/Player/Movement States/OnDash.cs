using UnityEngine;
using System.Collections;

public class OnDash : MovementState
{
    [SerializeField] private float dashForce;
    public float DashForce { get { return dashForce; } }

    [SerializeField] private float dashTime;
    private float timeLeft;

    [SerializeField]
    private float gravityModifier = 1f;

    [SerializeField]
    private int dashLimit = 3;
    public int DashLimit {  get { return dashLimit; } }

    private Vector2 direction;

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        //velocity.x += dashForce * direction.x * dashForce * Time.deltaTime;
        //velocity.y += dashForce * direction.y * dashForce * Time.deltaTime;

        velocity.y -= player.Gravity*Time.deltaTime*gravityModifier;

        if(controller.collisions.Left || controller.collisions.Right)
        {
            velocity.x = 0;
        }


        DecayExternalForces(ref externalForces);

        controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);


        if (timeLeft < 0)
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
        timeLeft -= Time.deltaTime;
        return null;
    }

    public override void OnEnter()
    {
        timeLeft = dashTime;
        //direction = input.leftStick;
        Rewired.Player p = input.inputPlayer;
        direction = Vector2.right*p.GetAxis("Aim Horizontal") + Vector2.up*p.GetAxis("Aim Vertical");
    }

    public override void OnExit()
    {
    }
}
