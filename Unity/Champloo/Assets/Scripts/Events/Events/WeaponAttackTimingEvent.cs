using System;

public class WeaponAttackTimingEvent : EventArgs
{
    public Weapon Target { get; set; }
    public TimingState Timing { get; set; }
}
