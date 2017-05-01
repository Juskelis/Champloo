using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineBottleProjectile : Projectile {
    [SerializeField]
    private GameObject explosion;

    protected override void ProcessHitPlayer(GameObject g)
    {
        Player p = g.gameObject.GetComponent<Player>()
                   ?? g.gameObject.GetComponentInParent<Player>();
        if (p == null) return;
        p.GetHit(this);
    }

    protected override void ProcessHitObstacle(Collider2D c)
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
