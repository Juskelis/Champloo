using System;

public class WeaponInHandEvent : EventArgs {
    public Weapon Target { get; set; }
}
