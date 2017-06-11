using UnityEngine;
using System.Collections;

public class Sword : Weapon {
    //private MeshRenderer ren;
    private BoxCollider2D col;

    [SerializeField]
    private SpriteRenderer visuals;

    private Animator anim;

    private Vector2 previousPosition;

    protected override void Start()
    {
        base.Start();
        
        //ren = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();

        //ren.enabled = alwaysVisible;
        col.enabled = false;
        visuals.enabled = alwaysVisible;
    }

    public override void Reset()
    {
        base.Reset();
        if (col != null) col.enabled = false;
        if (visuals != null) visuals.enabled = alwaysVisible;
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack)
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
        
        if (InHand)
        {
            InHand = false;
            Vector2 aim = OurPlayer.AimDirection;
            Projectile temp = (Projectile)Instantiate(
                thrownVersion,
                transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(aim),//input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                    transform.parent.forward
                )
            );
            temp.PlayerNumber = OurPlayer.PlayerNumber;
        }
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
