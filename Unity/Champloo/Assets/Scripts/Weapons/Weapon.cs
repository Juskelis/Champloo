using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
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
        Invoke("OnStart", startupTime);
    }

    protected virtual void OnStart()
    {
        Invoke("OnEnd", attackTime);
        attackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void OnEnd()
    {
        attackingState = TimingState.COOLDOWN;
        Invoke("OnCooledDown", cooldownTime);
    }

    public virtual void OnCooledDown()
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
        Invoke("OnStartSpecial", specialStartupTime);
    }

    protected virtual void OnStartSpecial()
    {
        Invoke("OnEndSpecial", specialTime);
        specialAttackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void OnEndSpecial()
    {
        specialAttackingState = TimingState.COOLDOWN;
        Invoke("OnCooledDownSpecial", specialCooldownTime);
    }

    protected virtual void OnCooledDownSpecial()
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
