using System;
using UnityEngine;
using System.Collections;

public class MovementState : MonoBehaviour
{
    protected Player player;
    protected Rewired.Player input;
    protected Controller2D controller;

    protected float externalForceDecay = 1f;

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        input = player.InputPlayer;//GetComponent<InputController>();
        controller = GetComponent<Controller2D>();
    }

    private void Update() { } //prevent children from using this

    public virtual MovementState UpdateState(ref Vector3 velocity, ref Vector3 externalForces) { return null; }

    public virtual void OnEnter() { }

    public virtual void OnExit() { }

    public virtual void DecayExternalForces(ref Vector3 externalForces)
    {
        //externalForces = Vector3.Lerp(externalForces, Vector3.zero, externalForceDecay);
        externalForces = Vector3.MoveTowards(externalForces, Vector3.zero, externalForceDecay);
    }
}
