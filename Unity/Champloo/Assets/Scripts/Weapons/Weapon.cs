using UnityEngine;
using UnityEngine.Networking;
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

    private bool isLocalPlayer = false;

    //[ClientCallback]
    protected virtual void Start()
    {
        InHand = true;
        isLocalPlayer = GetComponentInParent<NetworkIdentity>().isLocalPlayer;
    }

    //[ClientCallback]
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;
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
