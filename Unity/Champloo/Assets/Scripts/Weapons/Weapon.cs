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

    private int playerNumber;
    public int PlayerNumber { get; }

    [SerializeField]
    protected float startupTime;
    [SerializeField]
    protected float attackTime;
    [SerializeField]
    protected float cooldownTime;
    [SerializeField]
    protected float rechargeTime;

    [SerializeField]
    protected float specialStartupTime;
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float specialCooldownTime;
    [SerializeField]
    protected float specialRechargeTime;

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
        playerNumber = (GetComponent<Player>() ?? GetComponentInParent<Player>()).PlayerNumber;
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
        StartCoroutine(TimingCoroutine());
    }

    private IEnumerator TimingCoroutine()
    {
        attackingState = TimingState.WARMUP;
        yield return new WaitForSeconds(startupTime);
        OnStart();
        yield return new WaitForSeconds(attackTime);
        OnEnd();
        yield return new WaitForSeconds(cooldownTime);
        OnCooledDown();
        yield return null;
    }

    protected virtual IEnumerator RechargeCoroutine()
    {
        yield return null;
    }

    protected virtual void OnStart()
    {
        attackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void OnEnd()
    {
        attackingState = TimingState.COOLDOWN;
    }

    public virtual void OnCooledDown()
    {
        attackingState = TimingState.DONE;
    }

    public virtual void OnRecharge()
    {
        
    }

    public virtual void Special()
    {
        if (!CanAttack || !CanSpecialAttack)
        {
            return;
        }
        StartCoroutine(SpecialTimingCoroutine());
    }

    private IEnumerator SpecialTimingCoroutine()
    {
        specialAttackingState = TimingState.WARMUP;
        yield return new WaitForSeconds(specialStartupTime);
        OnStartSpecial();
        yield return new WaitForSeconds(specialTime);
        OnEndSpecial();
        yield return new WaitForSeconds(specialCooldownTime);
        OnCooledDownSpecial();
        yield return null;
    }

    protected virtual IEnumerator SpecialRechargeCoroutine()
    {
        yield return null;
    }

    protected virtual void OnStartSpecial()
    {
        specialAttackingState = TimingState.IN_PROGRESS;
    }

    protected virtual void OnEndSpecial()
    {
        specialAttackingState = TimingState.COOLDOWN;
    }

    protected virtual void OnCooledDownSpecial()
    {
        specialAttackingState = TimingState.DONE;
    }

    protected virtual void OnRechargeSpecial()
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
