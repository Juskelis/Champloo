using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    private bool inHand = false;
    public bool InHand {
        get {
            return inHand;
        }
        set {
            inHand = value;
            if(player != null)
            {
                player.FireEvent<WeaponInHandEvent>(new WeaponInHandEvent
                {
                    Target = this
                });
            }
        }
    }
    
    private bool attackCoroutineRunning = false;
    private bool specialAttackCoroutineRunning = false;

    protected TimingState attackingState = TimingState.DONE;
    public TimingState AttackState { get { return attackingState; } }
    protected TimingState specialAttackingState = TimingState.DONE;
    public TimingState SpecialAttackState { get { return specialAttackingState; } }

    public virtual bool CanAttack { get { return InHand && attackingState == TimingState.DONE; } }
    public virtual bool IsAttacking { get { return attackingState == TimingState.IN_PROGRESS; } }
    
    public virtual bool CanSpecialAttack { get { return InHand && specialAttackingState == TimingState.DONE; } }
    public virtual bool IsSpecialAttacking { get { return specialAttackingState == TimingState.IN_PROGRESS; } }

    private int playerNumber;
    public int PlayerNumber { get {return playerNumber;} }

    private Player player;
    public Player OurPlayer { get { return player; } }

    [SerializeField]
    protected float startupTime;
    [SerializeField]
    protected float attackTime;
    [SerializeField]
    protected float cooldownTime;
    [SerializeField]
    protected float rechargeTime;

    public float StartupTime { get { return startupTime; } }
    public float AttackTime { get { return attackTime; } }
    public float CooldownTime { get { return cooldownTime; } }
    public float RechargeTime { get { return rechargeTime; } }

    [SerializeField]
    protected float specialStartupTime;
    [SerializeField]
    protected float specialTime;
    [SerializeField]
    protected float specialCooldownTime;
    [SerializeField]
    protected float specialRechargeTime;
    
    public float SpecialStartupTime { get { return specialStartupTime; } }
    public float SpecialTime { get { return specialTime; } }
    public float SpecialCooldownTime { get { return specialCooldownTime; } }
    public float SpecialRechargeTime { get { return specialRechargeTime; } }

    [SerializeField]
    protected float projectileOffset;

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
        player = (GetComponent<Player>() ?? GetComponentInParent<Player>());
        playerNumber = player.PlayerNumber;
    }

    //[ClientCallback]
    protected virtual void Update()
    {
        if (!isLocalPlayer) return;
    }

    #region Attacking
    public virtual void Attack()
    {
        if (!CanAttack || !(specialAttackingState == TimingState.DONE || specialAttackingState == TimingState.RECHARGE))
        {
            return;
        }

        if (!attackCoroutineRunning)
        {
            attackCoroutineRunning = true;
            StartCoroutine(TimingCoroutine());
        }
    }

    private IEnumerator TimingCoroutine()
    {
        attackingState = TimingState.WARMUP;
        player.FireEvent(new WeaponAttackTimingEvent { Target = this, Timing = attackingState });
        if (startupTime > 0)
        {
            yield return new WaitForSeconds(startupTime);
        }
        OnStart();
        if (attackTime > 0)
        {
            yield return new WaitForSeconds(attackTime);
        }
        OnEnd();
        if (cooldownTime > 0)
        {
            yield return new WaitForSeconds(cooldownTime);
        }
        OnCooledDown();

        attackCoroutineRunning = false;
        yield return null;
    }

    protected virtual IEnumerator RechargeCoroutine()
    {
        yield return null;
    }

    protected virtual void OnStart()
    {
        attackingState = TimingState.IN_PROGRESS;
        player.FireEvent(new WeaponAttackTimingEvent {Target = this, Timing = attackingState});
    }

    protected virtual void OnEnd()
    {
        attackingState = TimingState.COOLDOWN;
        player.FireEvent(new WeaponAttackTimingEvent { Target = this, Timing = attackingState });
    }

    public virtual void OnCooledDown()
    {
        attackingState = TimingState.DONE;
        player.FireEvent(new WeaponAttackTimingEvent { Target = this, Timing = attackingState });
    }

    public virtual void OnRecharge()
    {
        
    }
    #endregion

    #region Special Attacking
    public virtual void Special()
    {
        if (!CanSpecialAttack || !(attackingState == TimingState.DONE || attackingState == TimingState.RECHARGE))
        {
            return;
        }
        if (!specialAttackCoroutineRunning)
        {
            specialAttackCoroutineRunning = true;
            StartCoroutine(SpecialTimingCoroutine());
        }
    }

    private IEnumerator SpecialTimingCoroutine()
    {
        specialAttackingState = TimingState.WARMUP;
        player.FireEvent(new WeaponSpecialTimingEvent() { Target = this, Timing = specialAttackingState });
        yield return new WaitForSeconds(specialStartupTime);
        OnStartSpecial();
        if (specialTime > 0)
        {
            yield return new WaitForSeconds(specialTime);
        }
        OnEndSpecial();
        if (specialCooldownTime > 0)
        {
            yield return new WaitForSeconds(specialCooldownTime);
        }
        OnCooledDownSpecial();
        if (specialRechargeTime > 0)
        {
            yield return new WaitForSeconds(specialRechargeTime);
        }
        OnRechargeSpecial();

        specialAttackCoroutineRunning = false;
        yield return null;
    }

    protected virtual IEnumerator SpecialRechargeCoroutine()
    {
        yield return null;
    }

    protected virtual void OnStartSpecial()
    {
        specialAttackingState = TimingState.IN_PROGRESS;
        player.FireEvent(new WeaponSpecialTimingEvent() { Target = this, Timing = specialAttackingState });
    }

    protected virtual void OnEndSpecial()
    {
        specialAttackingState = TimingState.COOLDOWN;
        player.FireEvent(new WeaponSpecialTimingEvent() { Target = this, Timing = specialAttackingState });
    }

    protected virtual void OnCooledDownSpecial()
    {
        specialAttackingState = TimingState.RECHARGE;
        player.FireEvent(new WeaponSpecialTimingEvent() { Target = this, Timing = specialAttackingState });
    }

    protected virtual void OnRechargeSpecial()
    {

        specialAttackingState = TimingState.DONE;
        player.FireEvent(new WeaponSpecialTimingEvent() { Target = this, Timing = specialAttackingState });
    }
    #endregion

    public virtual void PickUp()
    {
        if (!InHand)
        {
            InHand = true;
        }
    }

    public virtual void Reset()
    {
        InHand = true;
        StopAllCoroutines();
        attackCoroutineRunning = false;
        specialAttackCoroutineRunning = false;
        specialAttackingState = TimingState.DONE;
        attackingState = TimingState.DONE;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
    }
}
