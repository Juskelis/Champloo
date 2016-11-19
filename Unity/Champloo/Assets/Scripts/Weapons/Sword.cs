using UnityEngine;
using System.Collections;

public class Sword : Weapon {
    private Rewired.Player input;
    //private MeshRenderer ren;
    private BoxCollider2D col;

    [SerializeField]
    private SpriteRenderer visuals;

    

    protected override void Start()
    {
        base.Start();

        input = transform.parent.GetComponentInParent<Player>().InputPlayer;
        //ren = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();

        //ren.enabled = alwaysVisible;
        col.enabled = false;
        visuals.enabled = alwaysVisible;
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack)
        {
            //transform.parent.localScale = transform.parent.parent.localScale;
            Vector2 aim = Vector2.right*input.GetAxis("Aim Horizontal") + Vector2.up*input.GetAxis("Aim Vertical");
            //print(input.leftStickAngle);
            transform.parent.rotation = Quaternion.AngleAxis(
                Utility.Vector2AsAngle(aim),//input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                transform.parent.forward
            );
        }
    }

    public override void Special()
    {
        base.Special();
        
        if (InHand)
        {
            InHand = false;
            Vector2 aim = Vector2.right * input.GetAxis("Aim Horizontal") + Vector2.up * input.GetAxis("Aim Vertical");
            Projectile temp = (Projectile)Instantiate(
                thrownVersion,
                transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(aim),//input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                    transform.parent.forward
                )
            );
            temp.PlayerNumber = GetComponentInParent<Player>().PlayerNumber;
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();

        //ren.enabled = true;
        visuals.enabled = true;
        col.enabled = true;
    }

    protected override void EndAttack()
    {
        base.EndAttack();

        //ren.enabled = alwaysVisible;
        visuals.enabled = alwaysVisible;
        col.enabled = false;
    }
}
