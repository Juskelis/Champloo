using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{
    [SerializeField]
    [Range(1,4)]
    private int playerNumber = 1;

    public int PlayerNumber
    {
        get { return playerNumber; }
    }

    [SerializeField] private Transform spawnOnDeath;

    private InputController inputs;

    private Controller2D controller;

    private Weapon weapon;

    private Vector3 velocity = Vector3.zero;
    
    public float Gravity { get; set; }

    public float hitReactionTime;

    private MovementState movementState;
    //private Delegate movementState;

    private Weapon hitWith;

    void Start ()
    {
        movementState = GetComponent<OnGround>();

	    controller = GetComponent<Controller2D>();

        inputs = GetComponent<InputController>();
        inputs.playerNumber = playerNumber;

        weapon = GetComponentInChildren<Weapon>();
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
            velocity = inputs.leftStick.normalized * ((OnDash)next).DashForce;
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
        
        MovementState next = movementState.UpdateState(ref velocity);

        ChooseNextState(ref next);

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
            print("been hit " + transform.name);
            hitWith = otherWeapon;
            Invoke("GetHit", hitReactionTime);
        }
    }

    void GetHit()
    {
        hitWith = null;
        if(spawnOnDeath != null)
            Instantiate(spawnOnDeath, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }
}
