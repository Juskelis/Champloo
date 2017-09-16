using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownSwordProjectile : Projectile {
    [SerializeField]
    private Vector2 maxColliderSize;

    [SerializeField]
    private float growthTime = 10f;

    private Vector2 startColliderSize;
    private bool growing = false;
    private float startGrowTime;

    protected override void Update()
    {
        base.Update();

        if (!Moving)
        {
            if (!growing)
            {
                growing = true;
                startGrowTime = Time.time;
            }
            Vector3 size = Vector2.Lerp(startColliderSize, maxColliderSize, (Time.time - startGrowTime) / growthTime);
            ((BoxCollider2D) hitbox).size = size;
        }
    }

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
