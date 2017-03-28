using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public enum TimingState
    {
        WARMUP,
        IN_PROGRESS,
        COOLDOWN,
        DONE
    }

    public bool InHand { get; set; }

    protected TimingState attackingState = TimingState.DONE;
    public TimingState AttackState { get { return attackingState; } }
    protected TimingState specialAttackingState = TimingState.DONE;
    public TimingState SpecialAttackState { get { return specialAttackingState; } }
    
    public bool CanAttack { get { return InHand && attackingState == TimingState.DONE; } }
    public bool IsAttacking { get { return attackingState == TimingState.IN_PROGRESS; } }
    
    public bool CanSpecialAttack { get { return InHand && specialAttackingState == TimingState.DONE; } }
    public bool IsSpecialAttacking { get { return specialAttackingState == TimingState.IN_PROGRESS; } }

    [SerializeField]
    protected float startupTime;
    [SerializeField]
    protected float attackTime;
    [SerializeField]
    protected float cooldownTime;

    [SerializeField]
    protected float specialStartupTime;
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float specialCooldownTime;

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
    }

    public virtual void Attack()
    {
        if (!CanAttack || !CanSpecialAttack)
        {
            return;
        }
        attackingState = TimingState.WARMUP;
        Invoke("StartAttack", startupTime);
    }

    protected virtual void StartAttack()
    {
        Invoke("EndAttack", attackTime);
        attackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void EndAttack()
    {
        attackingState = TimingState.COOLDOWN;
        Invoke("EnableAttack", cooldownTime);
    }

    public virtual void EnableAttack()
    {
        attackingState = TimingState.DONE;
    }

    public virtual void Special()
    {
        if (!CanAttack || !CanSpecialAttack)
        {
            return;
        }
        specialAttackingState = TimingState.WARMUP;
        Invoke("StartSpecial", specialStartupTime);
    }

    protected virtual void StartSpecial()
    {
        Invoke("EndSpecial", specialTime);
        specialAttackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void EndSpecial()
    {
        specialAttackingState = TimingState.COOLDOWN;
        Invoke("EnableSpecial", specialCooldownTime);
    }

    protected virtual void EnableSpecial()
    {
        specialAttackingState = TimingState.DONE;
    }

    public virtual void PickUp()
    {
        if (!InHand)
        {
            InHand = true;
        }
    }
}
