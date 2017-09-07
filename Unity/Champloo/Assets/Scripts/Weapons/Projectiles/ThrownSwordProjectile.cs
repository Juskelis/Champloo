using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownSwordProjectile : Projectile {
    protected override void ProcessHitPlayer(GameObject g)
    {
        Player p = g.GetComponent<Player>()
                   ?? g.GetComponentInParent<Player>();
        if (p == null) return;
        if (!Moving || p.PlayerNumber == PlayerNumber || !(p.CurrentMovementState is InBlock))
        {
            base.ProcessHitPlayer(g);
        }
    }
}
