using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{
    [SerializeField]
    [Range(1,4)]
    private int playerNumber = 1;

    [SerializeField]
    private Color playerColor = Color.white;
    [SerializeField]
    private SpriteRenderer[] coloredSprites;
    [SerializeField]
    private Transform visuals;

    public int PlayerNumber
    {
        get { return playerNumber; }
    }

    [SerializeField] private Transform spawnOnDeath;

    private InputController inputs;
    private BoxCollider2D box;
    private Controller2D controller;
    private Animator anim;

    private Weapon weapon;

    [SerializeField]
    private float bounceForce = 10f;
    [SerializeField]
    private float deathForce = 20f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 externalForce = Vector3.zero;
    
    public float Gravity { get; set; }

    public float hitReactionTime;

    private MovementState movementState;
    public MovementState CurrentMovementState
    {
        get { return movementState; }
    }

    private Weapon hitWith;

    void Start ()
    {
        movementState = GetComponent<OnGround>();
        box = GetComponent<BoxCollider2D>();
	    controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();

        //change colors of child sprites
        foreach(SpriteRenderer s in coloredSprites)
        {
            s.color = playerColor;
        }

        inputs = GetComponent<InputController>();
        inputs.playerNumber = playerNumber;

        weapon = GetComponentInChildren<Weapon>();

        //attach to events
        controller.Crushed += Crushed;
        controller.Smashed += Smashed;
        controller.Stomped += StompedBy;
        controller.Bounced += Bounced;
    }

    void Crushed(object sender, EventArgs e)
    {
        FindObjectOfType<Score>().SubtractScore(playerNumber);
        Kill();
    }

    void Smashed(object sender, Player other)
    {
        ApplyForce(Vector3.up * bounceForce);
    }

    void StompedBy(object sender, Player other)
    {
        FindObjectOfType<Score>().AddScore(other.playerNumber);
        Kill();
    }

    void Bounced(object sender, Player other, bool horizontal)
    {
        float dir;
        if(horizontal)
        {
            dir = Mathf.Sign(transform.position.x - other.transform.position.x);
            ApplyForce(Vector3.right * dir * bounceForce);
        }
        else
        {
            dir = Mathf.Sign(transform.position.y - other.transform.position.y);
            ApplyForce(Vector3.up * dir * bounceForce, false);
        }
    }

    public void ApplyForce(Vector3 force, bool disableCollisions = true)
    {
        velocity = force;

        if (disableCollisions)
        {
            controller.collisions.above = false;
            controller.collisions.below = false;
        }
    }

    void Kill(Vector3 direction = default(Vector3))
    {
        velocity = Vector3.zero;
        externalForce = Vector3.zero;
        hitWith = null;

        if (spawnOnDeath != null)
        {
            Transform corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
            Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
            corpseBody.gravityScale = Gravity/Physics2D.gravity.magnitude;
            corpseBody.velocity = direction;
        }

        gameObject.SetActive(false);
    }

    void Destroy()
    {
        //detach from events
        controller.Crushed -= Crushed;
        controller.Smashed -= Smashed;
        controller.Stomped -= StompedBy;
        controller.Bounced -= Bounced;
    }

    //allows inherited classes to interfere with default FSM transitions
    //  by intercepting the desired next state before it reaches
    //  the end user
    //also lets user do polymorphic events for transitions, if need be.
    //  (note: only gives player scope)
    protected void ChooseNextState(ref MovementState next)
    {
        if(inputs.attack.Down && weapon.CanAttack && !(movementState is InAttack))
        {
            weapon.Attack();
            next = GetComponent<InAttack>();
        }
        else if(inputs.movementSpecial.Down && !(movementState is OnDash))
        {
            TrailRenderer tail = GetComponent<TrailRenderer>();
            tail.enabled = true;
            tail.Clear();
            next = GetComponent<OnDash>();
            Vector2 leftStickDir = inputs.leftStick.normalized;
            velocity = ((leftStickDir == Vector2.zero)?Vector2.up:leftStickDir) * ((OnDash)next).DashForce;
        }
        else if(inputs.taunt.Down && (movementState is OnGround))
        {
            next = GetComponent<TauntState>();
        }

        if (next != null)
        {
            if (movementState is OnDash)
            {
                GetComponent<TrailRenderer>().enabled = false;
            }
        }
    }

    void Update()
    {
        inputs.UpdateInputs();
        
        MovementState next = movementState.UpdateState(ref velocity, ref externalForce);

        ChooseNextState(ref next);

        //HandleCollisions();

        if (next != null)
        {
            movementState.OnExit();
            next.OnEnter();
            movementState = next;
        }

        //handle blocking/parrying
        if (hitWith != null)
        {
            if (weapon.InHand && inputs.block.Down)
            {
                hitWith = null;
            }
            else if (!weapon.InHand && inputs.parry.Down)
            {
                //steal weapon like a badass
                weapon.InHand = true;
                hitWith.InHand = false;
                hitWith = null;
            }
        }

        if (inputs.weaponSpecial.Down)
        {
            weapon.Special();
        }

        //update animation states
        if (anim != null)
        {
            float velMag = Mathf.Abs(velocity.x);
            anim.SetFloat("HorizontalSpeed", velMag);
            anim.SetBool("OnGround", movementState is OnGround);
            anim.SetBool("OnDash", movementState is OnDash);
            anim.SetBool("OnWall", movementState is OnWall);
            //anim.SetBool("InAttack", movementState is InAttack);
            anim.SetBool("InAir", movementState is InAir);
            anim.SetBool("Hit", hitWith != null);
            if (velMag > 0.01f)
            { 
                Vector3 localScale = visuals.localScale;
                localScale.x = Mathf.Sign(velocity.x);
                visuals.localScale = localScale;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //skip if the other object is inactive
        //  or if the other object is our child
        if (!col.gameObject.activeSelf || col.transform.IsChildOf(transform))
        {
            return;
        }

        Weapon otherWeapon = col.GetComponent<Weapon>();
        if (otherWeapon != null && hitWith == null)
        {
            hitWith = otherWeapon;
            Invoke("GetHit", hitReactionTime);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.activeSelf)
        {
            return;
        }

        Projectile p = col.gameObject.GetComponent<Projectile>();
        if(p != null)
        {
            if(p.PlayerNumber == playerNumber)
            {
                if (!p.Moving && !weapon.InHand)
                {
                    weapon.PickUp();
                    Destroy(p.gameObject);
                }
            }
            else if(p.Moving)
            {
                FindObjectOfType<Score>().AddScore(p.PlayerNumber);
                Kill(p.transform.right * deathForce);
            }
        }
    }

    void GetHit()
    {
        Kill(hitWith.transform.right * deathForce);
    }
}
