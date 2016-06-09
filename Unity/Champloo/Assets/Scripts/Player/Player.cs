﻿using System;
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
    private Vector3 externalForce = Vector3.zero;
    
    public float Gravity { get; set; }

    public float hitReactionTime;

    private MovementState movementState;
    public MovementState CurrentMovementState
    {
        get { return movementState; }
    }
    //private Delegate movementState;

    private Weapon hitWith;

    void Start ()
    {
        movementState = GetComponent<OnGround>();

	    controller = GetComponent<Controller2D>();

        inputs = GetComponent<InputController>();
        inputs.playerNumber = playerNumber;

        weapon = GetComponentInChildren<Weapon>();

        //attach to events
        controller.Crushed += Crushed;
        controller.Smashed += Smashed;
        controller.Stomped += StompedBy;
	}

    void Crushed(object sender, EventArgs e)
    {
        Kill();
    }

    void Smashed(object sender, Player other)
    {
        ApplyForce(Vector3.up * 30f);
    }

    void StompedBy(object sender, Player other)
    {
        Kill();
    }

    public void ApplyForce(Vector3 force)
    {
        externalForce += force;
    }

    void Kill()
    {
        velocity = Vector3.zero;
        externalForce = Vector3.zero;
        hitWith = null;

        if (spawnOnDeath != null)
            Instantiate(spawnOnDeath, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }

    void Destroy()
    {
        //detach from events
        controller.Crushed -= Crushed;
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
            hitWith = otherWeapon;
            Invoke("GetHit", hitReactionTime);
        }
    }

    void GetHit()
    {
        Kill();
    }
}
