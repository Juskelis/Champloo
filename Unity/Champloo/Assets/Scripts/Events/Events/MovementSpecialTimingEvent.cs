using System;

public class MovementSpecialTimingEvent : EventArgs {
    public MovementState Special { get; set; }
    public TimingState Timing { get; set; }
}
