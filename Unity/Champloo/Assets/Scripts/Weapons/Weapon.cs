using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
    public bool InHand { get; set; }

    public bool CanAttack { get { return InHand && !IsAttacking && reloadTimer <= 0; } }

    public bool IsAttacking { get { return attackTimer >= 0; } }

    [SerializeField]
    protected float attackTime;
    [SerializeField]
    protected float reloadTime;

    private float attackTimer;
    private float reloadTimer;


    [SerializeField]
    protected Projectile thrownVersion;

    public Projectile ThrownVersion { get { return thrownVersion; } }


    [SerializeField]
    protected bool alwaysVisible = false;


    protected virtual void Start()
    {
        InHand = true;
    }

    protected virtual void Update()
    {
        attackTimer -= Time.deltaTime;
        reloadTimer -= Time.deltaTime;
    }

    public virtual void Attack()
    {
        StartAttack();
        Invoke("EndAttack", attackTime);
    }

    protected virtual void StartAttack()
    {
        if (!InHand || attackTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        attackTimer = attackTime;
        reloadTimer = reloadTime;
    }

    protected virtual void EndAttack()
    {
    }

    public virtual void Special()
    {
    }

    public virtual void PickUp()
    {
        if (!InHand)
        {
            InHand = true;
        }
    }
}
