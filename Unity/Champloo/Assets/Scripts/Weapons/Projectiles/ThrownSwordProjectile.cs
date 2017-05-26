using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownSwordProjectile : Projectile {
    protected override void ProcessHitPlayer(GameObject g)
    {
        Player p = g.GetComponent<Player>()??g.GetComponentInParent<Player>();
        if (player == null || player.CurrentMovementState is InBlock) return;
        base.ProcessHitPlayer(g);
    }
}
