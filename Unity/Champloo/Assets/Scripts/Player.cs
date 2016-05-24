using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour
{

    private InputController inputs;

    private Controller2D controller;

    private Weapon weapon;

    private Vector3 velocity = Vector3.zero;
    
    public float Gravity { get; set; }

    private MovementState movementState;
    //private Delegate movementState;

    void Start ()
    {
        movementState = GetComponent<OnGround>();

	    controller = GetComponent<Controller2D>();

        inputs = GetComponent<InputController>();
        inputs.playerNumber = 1;

        weapon = GetComponent<Weapon>();
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
            velocity = inputs.leftStick * ((OnDash)next).DashForce;
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
    }
}
