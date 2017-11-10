using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WineBottleProjectile : Projectile
{
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private float stunTime = 1f;

    [SerializeField]
    private float degreesPerSecond = 45f;

    [SerializeField]
    private Vector3 offset;

    protected override void Update()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, -90f),
            degreesPerSecond * Time.deltaTime);

        base.Update();
    }

    protected override void ProcessHitPlayer(GameObject g)
    {
        Player p = g.gameObject.GetComponent<Player>()
                   ?? g.gameObject.GetComponentInParent<Player>();
        if (p == null || p.PlayerNumber == PlayerNumber || p.CurrentMovementState is InBlock) return;
        p.GetStunned(stunTime);

        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    protected override void ProcessHitObstacle(Collider2D c)
    {
        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        transform.position += offset;
    }
}
