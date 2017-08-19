﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBodyProjectile : Projectile
{

    [SerializeField]
    private float timer = 1f;
    protected override void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
        if (timer < 0)
            Destroy(gameObject);
    }

    protected override void ProcessHitPlayer(GameObject g)
    {
        Player p = g.GetComponent<Player>()
                   ?? g.GetComponentInParent<Player>();
        if (p == null) return;

        //deliberately not checking for blocking state from opponent
        if (p.PlayerNumber != PlayerNumber)
        {
            base.ProcessHitPlayer(g);
            Destroy(gameObject);
        }
    }
    protected override void ProcessHitObstacle(Collider2D c)
    {
        base.ProcessHitObstacle(c);
        Destroy(gameObject);
    }
}
