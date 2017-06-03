using System;

public class MovementStateChangedEvent : EventArgs {
    public MovementState Previous { get; set; }
    public MovementState Next { get; set; }
}
