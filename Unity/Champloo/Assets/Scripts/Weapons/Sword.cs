using UnityEngine;
using System.Collections;

public class Sword : Weapon {
    private InputController input;
    private SpriteRenderer ren;
    private BoxCollider2D col;

    [SerializeField]
    private Projectile thrownVersion;

    protected override void Start()
    {
        base.Start();

        input = GetComponentInParent<InputController>();
        ren = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();

        transform.rotation = Quaternion.AngleAxis(input.leftStickAngle, Vector3.forward);
    }

    public override void Special()
    {
        base.Special();
        
        if (InHand)
        {
            InHand = false;
            Projectile temp = (Projectile)Instantiate(thrownVersion, transform.position, transform.rotation);
            temp.PlayerNumber = GetComponentInParent<Player>().PlayerNumber;
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();

        ren.enabled = true;
        col.enabled = true;
    }

    protected override void EndAttack()
    {
        base.EndAttack();

        ren.enabled = false;
        col.enabled = false;
    }
}
