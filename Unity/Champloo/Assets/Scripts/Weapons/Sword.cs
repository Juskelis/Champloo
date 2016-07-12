using UnityEngine;
using System.Collections;

public class Sword : Weapon {
    private InputController input;
    private MeshRenderer ren;
    private BoxCollider2D col;

    [SerializeField]
    private Projectile thrownVersion;

    protected override void Start()
    {
        base.Start();

        input = transform.parent.GetComponentInParent<InputController>();
        ren = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack)
        {
            transform.parent.rotation = Quaternion.AngleAxis(
                input.leftStickAngle + 180 * transform.parent.localScale.x < 0?1:0,
                Vector3.forward
            );
        }
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
