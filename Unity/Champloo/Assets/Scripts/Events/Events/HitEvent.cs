using System;

public class HitEvent : EventArgs {
    public Player Hit { get; set; }
    public Player Attacker { get; set; }
}
