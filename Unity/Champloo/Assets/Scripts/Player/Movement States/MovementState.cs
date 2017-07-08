using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MovementState : NetworkBehaviour
{
    protected Player player;
    protected Rewired.Player input;
    protected InputController inputController;
    protected Controller2D controller;
    protected OnMovementSpecial movementSpecial;
    protected float externalForceDecay = 1f;

    public virtual bool AttackAllowed { get { return true; } }

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        input = player.InputPlayer;
        inputController = GetComponent<InputController>();
        controller = GetComponent<Controller2D>();
        movementSpecial = GetComponent<OnMovementSpecial>();
    }

    private void Update() { } //prevent children from using this

    /// <summary>
    /// Called when entering a state 
    /// </summary>
    /// <param name="inVelocity">input velocity</param>
    /// <param name="inExternalForces">input external forces</param>
    /// <param name="outVelocity">output velocity</param>
    /// <param name="outExternalForces">output external forces</param>
    /// <remarks>In/Out style used to guarantee caller has authority on variables</remarks>
    public virtual void OnEnter(
        Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        outVelocity = inVelocity;
        outExternalForces = inExternalForces;
    }

    public virtual void OnExit(
        Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        outVelocity = inVelocity;
        outExternalForces = inExternalForces;
    }

    /// <summary>
    /// Decays the Vector3 passed in and then returns the result
    /// </summary>
    /// <param name="externalForces">The external force to decay</param>
    /// <returns>The decayed external force</returns>
    /// <remarks>This function behaves with Time.deltaTime</remarks>
    public virtual Vector3 DecayExternalForces(Vector3 externalForces)
    {
        return Vector3.MoveTowards(externalForces, Vector3.zero, externalForceDecay);
    }

    /// <summary>
    /// Decays the Vector3 passed in and then returns the result
    /// </summary>
    /// <param name="velocity">The velocity to diminish</param>
    /// <returns>The diminished velocity</returns>
    public virtual Vector3 ApplyFriction(Vector3 velocity) { return velocity; }

    /// <summary>
    /// Applies inputs to the input velocity/externalforces, and then outputs them 
    /// </summary>
    /// <param name="inVelocity">input velocity</param>
    /// <param name="inExternalForces">input external forces</param>
    /// <param name="outVelocity">output velocity</param>
    /// <param name="outExternalForces">output external forces</param>
    /// <remarks>In/Out style used to guarantee caller has authority on variables</remarks>
    public virtual void ApplyInputs(
        Vector3 inVelocity, Vector3 inExternalForces,
        out Vector3 outVelocity, out Vector3 outExternalForces)
    {
        outVelocity = inVelocity;
        outExternalForces = inExternalForces;
    }

    /// <summary>
    /// Determines next MovementState based on player's current overall state
    /// </summary>
    /// <param name="velocity">The velocity the player is moving at</param>
    /// <param name="externalForces">The external forces the player is experiencing</param>
    /// <returns>The next movement state, or null if we stay in this state</returns>
    public virtual MovementState DecideNextState(Vector3 velocity, Vector3 externalForces) { return null; }
}
