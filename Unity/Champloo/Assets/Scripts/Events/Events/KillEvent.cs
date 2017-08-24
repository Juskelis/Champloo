using System;

public class KillEvent : EventArgs {
    public Player Victim { get; set; }
    public Player Killer { get; set; }
    public Weapon MurderWeapon { get; set; }
    public Projectile MurderProjectile { get; set; }
}
