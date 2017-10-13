using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimeSword : Weapon {

    private BoxCollider2D col;

    [SerializeField]
    private SpriteRenderer visuals;

    [SerializeField]
    private Vector2 superSwordSize = Vector2.one;

    private Vector3 defaultTransformScale;

    public override bool IsSpecialAttacking
    {
        get { return base.IsSpecialAttacking || specialAttackingState == TimingState.WARMUP; }
    }

    public override void Reset()
    {
        base.Reset();
        if (col != null) col.enabled = false;
        if (visuals != null) visuals.enabled = alwaysVisible;
        transform.parent.localScale = defaultTransformScale;
    }

    private void Awake()
    {
        defaultTransformScale = transform.parent.localScale;
    }

    protected override void Start()
    {
        base.Start();

        //ren = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();

        //ren.enabled = alwaysVisible;
        col.enabled = false;
        visuals.enabled = alwaysVisible;
    }

    protected override void Update()
    {
        base.Update();
        if ((CanAttack && CanSpecialAttack) || specialAttackingState == TimingState.WARMUP)
        {
            //transform.parent.localScale = transform.parent.parent.localScale;
            Vector2 aim = OurPlayer.AimDirection;
            //print(input.leftStickAngle);
            transform.parent.rotation = Quaternion.AngleAxis(
                Utility.Vector2AsAngle(aim),//input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                transform.parent.forward
            );
        }
    }

    protected override void OnStartSpecial()
    {
        base.OnStartSpecial();

        transform.parent.localScale = Vector2.Scale(defaultTransformScale, superSwordSize);

        visuals.enabled = true;
        col.enabled = true;
    }

    protected override void OnEndSpecial()
    {
        base.OnEndSpecial();

        transform.parent.localScale = defaultTransformScale;

        visuals.enabled = alwaysVisible;
        col.enabled = false;
    }

    protected override void OnStart()
    {
        base.OnStart();

        //ren.enabled = true;
        visuals.enabled = true;
        col.enabled = true;
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        //ren.enabled = alwaysVisible;
        visuals.enabled = alwaysVisible;
        col.enabled = false;
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
