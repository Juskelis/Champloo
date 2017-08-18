using System;
using UnityEngine;

public class ProjectileHitEvent : EventArgs {
    public Player HitPlayer { get; set; }
    public GameObject HitGameObject { get; set; }
}
