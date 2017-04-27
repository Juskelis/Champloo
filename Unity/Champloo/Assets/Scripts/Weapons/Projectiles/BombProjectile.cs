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
        Invoke("Explode", fuseLength);
    }

    protected override void Update()
    {
        speed = Mathf.Max(0, speed - Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, -90f),
            45f * Time.deltaTime);
        base.Update();
    }

    protected override void ProcessHitPlayer(GameObject o)
    {
        //turn off parent functionality
    }

    protected override void ProcessHitObstacle(Collider2D c)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.right,
            Mathf.Infinity,
            obstacleMask);
        Vector3 reflection = Vector3.Reflect(transform.right, hit.normal);
        Vector3 bounce = reflection.normalized;
        float angleToBounce = Vector3.Angle(transform.right, bounce);
        transform.Rotate(0, 0, -angleToBounce);
    }

    private void Explode()
    {
        //go through all players and push them away
        foreach (Player player in FindObjectsOfType<Player>())
        {
            Vector3 pushDir = (player.transform.position - transform.position).normalized;
            player.ApplyForce(pushDir * explosionForce);
        }

        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
