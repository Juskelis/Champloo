using UnityEngine;
using System.Collections;

public class OnTether : OnMovementSpecial
{
    [SerializeField]
    private float tetherForce;
    public float TetherForce { get { return tetherForce; } }

    [SerializeField]
    private float tetherTime;
    private float timeLeft;

    [SerializeField]
    private float gravityModifier = 1f;

    private Weapon weapon;
    private Projectile temp;

    private Vector2 direction;
    private Vector3 tetherVelocity;

    private bool isThrown;

    private Collider2D col;
    private MovementState currentState;

    TrailRenderer tail;

    protected override void Start()
    {
        base.Start();
        tail = GetComponent<TrailRenderer>();
        weapon = GetComponentInChildren<Weapon>();
    }

    public override MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces)
    {
        col = GetComponent<Collider2D>();

        //check to see if the tether has been thrown
        if (isThrown)
        {
            //check if temp exists, and if so, is it moving
            if (temp && !temp.Moving)
            {
                //find the angle between the player and the stopped weapon
                //then pull the player continuously towards the tether
                Vector2 pullDirection = temp.transform.position - player.transform.position;
                velocity = pullDirection * tetherForce;

                //check for collisions in the x  and y directions that the player is moving
                if ((controller.collisions.Left && direction.x < 0) || (controller.collisions.Right && direction.x > 0))
                {
                    velocity.x = 0;
                }
                if ((controller.collisions.Above && direction.y > 0) || (controller.collisions.Below && direction.x < 0))
                {
                    velocity.y = 0;
                }

                DecayExternalForces(ref externalForces);
                controller.Move(velocity * Time.deltaTime + externalForces * Time.deltaTime);
            }
            //If no on existing, exit the state
            if (!temp)
            {
                return checkState();
            }

            //otherwise update the state as it would if the tether wasn't thrown
            else
            {
                currentState = checkState();
                currentState = currentState.UpdateState(ref velocity, ref externalForces);
            }
        }
        //if not thrown, exit state
        else
        {
            return checkState();
        }

        timeLeft -= Time.deltaTime;
        return null;
    }

    public override void OnEnter(ref Vector3 velocity, ref Vector3 externalForces)
    {
        Rewired.Player p = input.inputPlayer;

        //set up the tether visual trail
        tail.enabled = true;
        tail.Clear();

        //actually throw the weapon
        if (weapon.InHand && !isDisabled)
        {
            isDisabled = true;
            weapon.InHand = false;
            isThrown = true;

            Vector2 aim = Vector2.right * input.inputPlayer.GetAxis("Aim Horizontal") + Vector2.up * input.inputPlayer.GetAxis("Aim Vertical");
            temp = (Projectile)Instantiate(
                weapon.ThrownVersion,
                weapon.transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(aim),
                    weapon.transform.parent.forward
                )
            );
            
            temp.PlayerNumber = player.PlayerNumber;
        }
        else if (!weapon.InHand)
        {
            isThrown = false;
        }

    }

    public override void OnExit(ref Vector3 velocity, ref Vector3 externalForces)
    {
        tail.enabled = false;
        isThrown = false;
        isDisabled = false;
    }

    private MovementState checkState()
    {
        //this probably needs to be changed so that it doesn't trigger on enemy players being below or to the side
        if (controller.collisions.Below)
        {
            return GetComponent<OnGround>();
        }
        else if (controller.collisions.Left || controller.collisions.Right)
        {
            return GetComponent<OnWall>();
        }
        else
        {
            return GetComponent<InAir>();
        }
    }

}