using UnityEngine;

public class Axe : Weapon
{
    private BoxCollider2D col;

    [SerializeField]
    private SpriteRenderer visuals;

    [SerializeField]
    private float swingAngle = 90f;

    [SerializeField]
    private float startAngleOffset = 45f;

    [SerializeField]
    private AnimationCurve angleOverTime;

    private float startAngle;
    private float endAngle;
    private float startTime;

    public override void Reset()
    {
        base.Reset();
        if (col != null) col.enabled = false;
        if (visuals != null) visuals.enabled = alwaysVisible;
    }

    protected override void Start()
    {
        base.Start();
        
        col = GetComponent<BoxCollider2D>();
        
        col.enabled = false;
        visuals.enabled = alwaysVisible;
    }

    protected override void OnStart()
    {
        base.OnStart();
        
        visuals.enabled = true;
        col.enabled = true;

        if (Mathf.Cos(transform.parent.rotation.eulerAngles.z * Mathf.Deg2Rad) > 0)
        {
            //we're attacking to the right
            startAngle = transform.parent.rotation.eulerAngles.z + startAngleOffset;
            endAngle = startAngle - swingAngle;
        }
        else
        {
            //we're attacking to the left
            startAngle = transform.parent.rotation.eulerAngles.z - startAngleOffset;
            endAngle = startAngle + swingAngle;
        }
        startTime = Time.time;

        transform.parent.rotation = Quaternion.Euler(0,0,startAngle);
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        
        visuals.enabled = alwaysVisible;
        col.enabled = false;
    }

    protected override void OnStartSpecial()
    {
        base.OnStartSpecial();

        visuals.enabled = true;
        col.enabled = true;
        Vector2 aim = OurPlayer.DeadzoneAimDirection;
        Vector3 aimAlt = new Vector3(aim.x, aim.y);
        Projectile temp = (Projectile)Instantiate(
            thrownVersion,
            OurPlayer.CenterOfSprite + aimAlt.normalized * projectileOffset,
            Quaternion.AngleAxis(
                Utility.Vector2AsAngle(aim),//input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                transform.parent.forward
                )
            );
        temp.PlayerNumber = OurPlayer.PlayerNumber;
    }
    

    protected override void OnEndSpecial()
    {
        base.OnEndSpecial();

        visuals.enabled = alwaysVisible;
        col.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack || CanSpecialAttack)
        {
            Vector2 aim = OurPlayer.AimDirection;
            transform.parent.rotation = Quaternion.AngleAxis(
                Utility.Vector2AsAngle(aim),
                transform.parent.forward
            );
        }
        if (IsAttacking)
        {
            float percentThrough = angleOverTime.Evaluate((Time.time - startTime) / attackTime);
            float lerpedAngle = Mathf.LerpAngle(startAngle, endAngle, percentThrough);
            transform.parent.rotation = Quaternion.Euler(0, 0, lerpedAngle);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
        Player p = col.GetComponent<Player>();
        p = p ?? col.GetComponentInParent<Player>();
        if (p != null)
        {
            p.GetHit(this);
        }
    }
}
