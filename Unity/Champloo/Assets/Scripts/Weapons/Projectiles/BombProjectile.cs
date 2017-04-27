using UnityEngine;

public class BombProjectile : Projectile {
    [SerializeField]
    private float fuseLength;

    [SerializeField]
    private float explosionForce;

    [SerializeField]
    private Transform explosion;

    private float fuseTimeLeft = 0f;

    protected override void Awake()
    {
        base.Awake();
        fuseTimeLeft -= Time.deltaTime;
    }

    protected override void ProcessHitPlayer(GameObject o)
    {
        //turn off parent functionality
    }

    protected override void ProcessHitObstacle(Collider2D c)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.forward,
            Mathf.Infinity,
            obstacleMask);
        Vector3 bounce = -(Vector3.Reflect(transform.forward, hit.normal).normalized);
        float angleToBounce = Vector3.Angle(transform.forward, bounce);
        transform.Rotate(0, 0, angleToBounce);
    }
}
