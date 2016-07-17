﻿using UnityEngine;
using System.Collections;

public class Sword : Weapon {
    private InputController input;
    private MeshRenderer ren;
    private BoxCollider2D col;

    [SerializeField]
    private Projectile thrownVersion;
    [SerializeField]
    private bool alwaysVisible = false;

    protected override void Start()
    {
        base.Start();

        input = transform.parent.GetComponentInParent<InputController>();
        ren = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();

        ren.enabled = alwaysVisible;
        col.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (CanAttack)
        {
            //transform.parent.localScale = transform.parent.parent.localScale;

            //print(input.leftStickAngle);
            transform.parent.rotation = Quaternion.AngleAxis(
                input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
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
            Projectile temp = (Projectile)Instantiate(
                thrownVersion,
                transform.position,
                Quaternion.AngleAxis(
                    input.leftStickAngle,//transform.parent.parent.localScale.x < 0 ? 180 - input.leftStickAngle : input.leftStickAngle,
                    transform.parent.forward
                )
            );
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
        
        ren.enabled = alwaysVisible;
        col.enabled = false;
    }
}
