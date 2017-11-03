using System;
using UnityEngine;

public class BombHands : Weapon
{
    [SerializeField]
    private Transform explosion;

    [SerializeField]
    private SpriteRenderer visuals;

    private BoxCollider2D col;

    private bool bombOut;

    private void OnHit(object sender, EventArgs args)
    {
        if (specialAttackingState == TimingState.IN_PROGRESS)
        {
            HitEvent e = (HitEvent) args;
            e.Hit.CancelHit();
            e.Attacker.GetHit(this, true);
            bombOut = false;
            Instantiate(explosion, e.Attacker.CenterOfSprite, transform.rotation);
            OnEndSpecial();
        }
    }

    private void CheckBomb()
    {
        if (bombOut)
        {
            OurPlayer.Suicide();
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    protected override void Start()
    {
        base.Start();
        GetComponentInParent<LocalEventDispatcher>().AddListener<HitEvent>(OnHit);

        col = GetComponent<BoxCollider2D>();
        
        col.enabled = false;
        visuals.enabled = alwaysVisible;
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack)
        {
            Vector2 aim = OurPlayer.AimDirection;
            transform.parent.rotation = Quaternion.AngleAxis(
                Utility.Vector2AsAngle(aim),
                transform.parent.forward
            );
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        
        visuals.enabled = true;
        col.enabled = true;
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
        bombOut = true;
    }

    protected override void OnEndSpecial()
    {
        //lets us skip over this if we've already ended manually
        if (specialAttackingState == TimingState.IN_PROGRESS)
        {
            base.OnEndSpecial();
            CheckBomb();
        }
        bombOut = false;
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

    public override void Reset()
    {
        base.Reset();
        if (col != null) col.enabled = false;
        if (visuals != null) visuals.enabled = alwaysVisible;
        bombOut = false;
    }
}
