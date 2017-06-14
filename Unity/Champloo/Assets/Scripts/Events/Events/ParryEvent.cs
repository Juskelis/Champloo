using System;

public class ParryEvent : EventArgs
{
    public Player Parrier { get; set; }
    public Player Attacker { get; set; }
}
