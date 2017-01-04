using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
    public bool InHand { get; set; }

    public bool CanAttack { get { return InHand && !IsAttacking && cooldownTimeLeft <= 0; } }

    public bool IsAttacking { get { return attackTimeLeft >= 0; } }

    [SerializeField]
    protected float startupTime;
    [SerializeField]
    protected float attackTime;
    [SerializeField]
    protected float cooldownTime;

    private float startupTimeLeft;
    private float attackTimeLeft;
    private float cooldownTimeLeft;


    [SerializeField]
    protected Projectile thrownVersion;

    public Projectile ThrownVersion { get { return thrownVersion; } }


    [SerializeField]
    protected bool alwaysVisible = false;

    private bool isLocalPlayer = false;
    protected virtual void Start()
    {
        InHand = true;
        isLocalPlayer = GetComponentInParent<NetworkIdentity>().isLocalPlayer;
    }

    //[ClientCallback]
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;
        attackTimeLeft -= Time.deltaTime;
        cooldownTimeLeft -= Time.deltaTime;

        //Debug.Log("cooldown time left: " + cooldownTimeLeft);
    }

    public virtual void Attack()
    {
        StartAttack();
        Invoke("EndAttack", attackTime);
    }

    protected virtual void StartAttack()
    {
        if (!InHand || cooldownTimeLeft > 0)
        {
            return;
        }

        attackTimeLeft = attackTime;
        cooldownTimeLeft = cooldownTime;
        
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
