using System;
using UnityEngine;

public class JumpEvent : EventArgs {
    public MovementState Active { get; set; }
    public Vector3 Direction { get; set; }
}
