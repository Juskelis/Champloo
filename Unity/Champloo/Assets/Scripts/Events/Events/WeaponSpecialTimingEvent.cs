using System;

public class WeaponSpecialTimingEvent : EventArgs
{
    public Weapon Target { get; set; }
    public TimingState Timing { get; set; }
}
