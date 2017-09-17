using UnityEngine;

public class Lance : Projectile {

    [Header("Velocity settings")]

    [SerializeField]
    private float minSize = 1f;

    [SerializeField]
    private float maxSize = 2f;

    [SerializeField]
    private float maxPlayerVelocity = 10f;
    
    [SerializeField]
    private AnimationCurve sizeByValocityCurve;

    [SerializeField]
    private float duration;

    protected override void Start()
    {
        base.Start();
        transform.position = OurPlayer.transform.position;
        transform.SetParent(OurPlayer.transform, true);
        Invoke("Finish", duration);
    }

    protected override void Update()
    {
        Vector3 velocity = OurPlayer.Velocity;
        transform.rotation = Quaternion.AngleAxis(
                Utility.Vector2AsAngle(velocity),
                transform.parent.forward
            );
        Vector3 localscale = transform.localScale;
        localscale.x =
            sizeByValocityCurve.Evaluate(
                OurPlayer.Velocity.magnitude/maxPlayerVelocity
            )*(maxSize - minSize) + minSize;
        transform.localScale = localscale;
    }

    protected override void ProcessHitObstacle(Collider2D c)
    {
        //do nothing on purpose
    }

    void Finish()
    {
        OurPlayer.GetComponentInChildren<Weapon>().PickUp();
        Destroy(gameObject);
    }
}
