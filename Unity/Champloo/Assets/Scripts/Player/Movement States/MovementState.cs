using System;
using UnityEngine;
using System.Collections;

public class MovementState : MonoBehaviour
{
    protected Player player;
    protected InputController input;
    protected Controller2D controller;

    protected virtual void Start()
    {
        player = GetComponent<Player>();
        input = GetComponent<InputController>();
        controller = GetComponent<Controller2D>();
    }

    private void Update() { } //prevent children from using this

    public virtual MovementState UpdateState(ref Vector3 velocity) { return null; }

    public virtual void OnEnter() { }

    public virtual void OnExit() { }
}
