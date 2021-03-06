﻿using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class Player : NetworkBehaviour
{
    #region Variables

    #region State Variables
    [SyncVar(hook = "OnDeath")]
    private bool dead = false;
    private bool localDead = false; //used to prevent network race conditions
    public bool Dead
    {
        get { return dead || localDead; }
    }
    
    private int playerNumber = 1;
    public int PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

    private int ourNetworkID;
    
    private MovementState movementState;
    public MovementState CurrentMovementState
    {
        get { return movementState; }
    }

    private bool stunned = false;
    public bool Stunned { get { return stunned; } }

    public int HorizontalDirection
    {
        get { return (int)visuals.localScale.x; }
    }

    public Vector2 AimDirection
    {
        get
        {
            Vector2 aim = Vector2.right * inputPlayer.GetAxis("Aim Horizontal") + Vector2.up * inputPlayer.GetAxis("Aim Vertical");
            if (aim.sqrMagnitude <= 0)
            {
                //default to forward
                aim.x = movementState is OnWall ? -HorizontalDirection : HorizontalDirection;
                aim.y = 0;
            }
            return aim;
        }
    }

    public Vector2 DeadzoneAimDirection
    {
        get { return inputs.ApplyDeadZone(AimDirection); }
    }

    #endregion

    #region Customization Variables

    [SerializeField]
    private Color playerColor = Color.white;
    public Color PlayerColor { get { return playerColor; } }

    [SerializeField]
    private SpriteRenderer[] coloredSprites;
    public SpriteRenderer[] ColoredSprites
    {
        get { return coloredSprites; }
    }

    [SerializeField]
    private Transform visuals;

    [SerializeField]
    private Transform centerOfSprite;

    [SerializeField]
    private Transform hitbox;

    [SerializeField]
    private float bounceForce = 10f;

    [Space]
    [SerializeField]
    public float maxJumpHeight = 4;

    [SerializeField]
    public float minJumpHeight = 2;

    [SerializeField]
    public float Gravity = 50;

    public float hitReactionTime;

    #endregion

    #region Utility Variables

    private Rewired.Player inputPlayer;
    public Rewired.Player InputPlayer
    {
        get
        {
            if (inputPlayer == null)
            {
                inputPlayer = Utility.GetNetworkPlayer(GetComponent<NetworkIdentity>().playerControllerId);
            }
            return inputPlayer;
        }
        private set { inputPlayer = value; }
    }

    [SyncVar(hook = "OnVelocity")]
    private Vector3 velocity = Vector3.zero;

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    [SyncVar(hook = "OnExternalForce")]
    private Vector3 externalForce = Vector3.zero;

    //public float Gravity { get; set; }

    private Weapon hitWith;
    private Projectile hitWithProjectile;

    private bool manuallyUpdatedDirection = false;

    private bool invincible = false;

    public Vector3 CenterOfSprite
    {
        get { return centerOfSprite.position; }
    }

    public Vector3 CenterOfPlayer
    {
        get { return transform.position; }
    }

    #endregion

    #region Component Variables

    private InputController inputs;
    private BoxCollider2D box;
    private BoxCollider2D hitbox_collider;
    private Controller2D controller;
    private Animator anim;

  
    public Weapon Weapon { get { return weapon; } set { } }
    private Weapon weapon;
    private Shield shield;

    private OnMovementSpecial movementSpecial;

    private SpriteRenderer currentSprite;

    private LocalEventDispatcher dispatcher;
    #endregion

    #endregion

    #region Initialization

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        dispatcher = gameObject.GetComponent<LocalEventDispatcher>();
    }

    //[ClientCallback]
    public void Start ()
    {
        weapon = GetComponentInChildren<Weapon>();
        shield = GetComponentInChildren<Shield>();
        movementState = GetComponent<OnGround>();
        movementSpecial = GetComponent<OnMovementSpecial>();
        box = GetComponent<BoxCollider2D>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
        hitbox_collider = hitbox.GetComponent<BoxCollider2D>();

        inputs = GetComponent<InputController>();

        ourNetworkID = (int)GetComponent<NetworkIdentity>().netId.Value;
        int ourPlayerControllerID = GetComponent<NetworkIdentity>().playerControllerId;
        //playerNumber = Utility.GetLocalPlayerNumber(ourPlayerControllerID);
        playerNumber = ourNetworkID;
        InputPlayer = Utility.GetNetworkPlayer(ourPlayerControllerID);
        //transform.position = Random.insideUnitCircle;
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);

        OnDeathChanged(false);

        weapon.PickUp();

        //change colors of child sprites
        playerColor = GetComponent<PlayerSettings>().Color;
        foreach(SpriteRenderer s in coloredSprites)
        {
            s.color = playerColor;
        }
        //inputs.playerNumber = playerNumber;

        //attach to events
        controller.Crushed += Crushed;
        controller.Collision += Collided;
        SceneManager.sceneLoaded += OnSceneLoaded;

        

        currentSprite = visuals.GetChild(0).GetComponent<SpriteRenderer>();
        //add to score register
        Score.AddPlayer(ourNetworkID);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode loadSceneMode)
    {
        Reset();
    }

    void Reset()
    {
        Spawn();
        Score.AddPlayer(ourNetworkID);
    }

    #endregion

    #region De-Initialization

    void OnDestroy()
    {
        //detach from events
        controller.Crushed -= Crushed;
        controller.Collision -= Collided;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //detach from score register
        Score.RemovePlayer(ourNetworkID);
    }
        
    #endregion
        
    #region Spawning

    private void Spawn()
    {
        CancelInvoke("Spawn");
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);
        gameObject.SetActive(true);
        OnDeathChanged(false);
        hitWith = null;
        hitWithProjectile = null;
        manuallyUpdatedDirection = false;
        stunned = false;
        invincible = false;
        OnVelocityChanged(Vector3.zero);
        OnExternalForceChanged(Vector3.zero);
        Vector3 garbageVelocity;
        Vector3 garbageExternalForces;
        foreach (MovementState state in GetComponentsInChildren<MovementState>())
        {
            state.Reset();
        }
        movementState.OnExit(velocity, externalForce, out garbageVelocity, out garbageExternalForces);
        movementState = GetComponent<OnGround>();
        movementState.OnEnter(velocity, externalForce, out garbageVelocity, out garbageExternalForces);
        CmdUpdateMovementState(movementState.GetType().ToString());
        weapon.Reset();
        shield.Reset();
        foreach (SpriteRenderer s in coloredSprites)
        {
            s.color = playerColor;
        }
    }

    #endregion

    #region Death

    [Command]
    void CmdKill(Vector3 direction)
    {
        RpcKilled(direction);
    }

    //[Client]
    private void Kill(Vector3 direction = default(Vector3))
    {
        if (isLocalPlayer && hasAuthority && !Dead)
        {
            localDead = true;
            CmdKill(direction);
        }
    }

    [ClientRpc]
    void RpcKilled(Vector3 direction)
    {
        if (dead) return;
        FireEvent(new DeathEvent { deadPlayer = this });

        OnVelocityChanged(Vector3.zero);
        OnExternalForceChanged(Vector3.zero);

        hitWith = null;

        gameObject.SetActive(false);
        OnDeathChanged(true);

        float time = FindObjectOfType<PlayerSpawner>().SpawnTime;
        Invoke("Spawn", time);
    }

    public void Suicide()
    {
        if (Dead) return;
        FindObjectOfType<Score>().SubtractScore(playerNumber);
        Kill();
    }

    #endregion

    #region Collision Callbacks
    [Client]
    void Crushed(object sender, GameObject obj)
    {
        
        if (obj.GetComponent<Player>() != null || Dead) return;
        FindObjectOfType<Score>().SubtractScore(playerNumber);
        Kill();
        
    }

    void Collided(object sender, GameObject other, Controller2D.CollisionInfo info)
    {
        Player otherPlayer = other.GetComponent<Player>();
        if (otherPlayer == null || otherPlayer.Dead || Dead) return;

        bool killThem = false;
        bool killUs = false;

        bool usInKillerDash = movementState is OnKillerDash &&
            movementSpecial.Progress == TimingState.IN_PROGRESS;
        bool themInKillerDash = otherPlayer.movementState is OnKillerDash &&
            otherPlayer.movementSpecial.Progress == TimingState.IN_PROGRESS;

        bool usBlocking = movementState is InBlock;
        bool themBlocking = otherPlayer.movementState is InBlock;

        bool usInvulnerable = invincible;
        bool themInvulnerable = otherPlayer.invincible;

        if (usInKillerDash && !themInKillerDash && !themBlocking && !themInvulnerable)
        {
            killThem = true;
        }
        else if (themInKillerDash && !usInKillerDash && !usBlocking && !usInvulnerable)
        {
            killUs = true;
        }
        else if (info.Below)
        {
            ApplyForce(
                (CenterOfPlayer - otherPlayer.CenterOfPlayer).normalized * bounceForce);
            FireEvent(new BounceEvent { A = this, B = otherPlayer });
            if (movementState is OnDash && !otherPlayer.Dead)
            {
                //killThem = true;
            }
        }
        //if another player is above
        else if (info.Above)
        {
            otherPlayer.ApplyForce(
                (otherPlayer.CenterOfPlayer - CenterOfPlayer).normalized * otherPlayer.bounceForce);
            FireEvent(new BounceEvent { A = this, B = otherPlayer });
            if (otherPlayer.movementState is OnDash && !Dead)
            {
                //killUs = true;
            }
        }
        else
        {
            //apply forces no matter what
            ApplyForce(
                (CenterOfPlayer - otherPlayer.CenterOfPlayer).normalized * bounceForce);
            otherPlayer.ApplyForce(
                (otherPlayer.CenterOfPlayer - CenterOfPlayer).normalized * otherPlayer.bounceForce);
            FireEvent(new BounceEvent { A = this, B = otherPlayer });
        }

        if (killThem)
        {
            FireEvent(new KillEvent { Killer = this, Victim = otherPlayer });
            otherPlayer.Kill(Vector3.down);
            CmdScore();
        }
        if (killUs)
        {
            FireEvent(new KillEvent { Killer = otherPlayer, Victim = this });
            Kill(Vector3.down);
            otherPlayer.CmdScore();
        }
    }

    [ClientCallback]
    void OnTriggerEnter2D(Collider2D col)
    {
        //skip if the other object is inactive
        //  or if the other object is our child
        if (!col.gameObject.activeSelf || col.transform.IsChildOf(transform))
        {
            return;
        }
        /*
        Projectile p = col.GetComponent<Projectile>();
        if (p != null && !p.Moving && !weapon.InHand && p.PlayerNumber == playerNumber)
        {
            weapon.PickUp();
            Destroy(p.gameObject);
        }
        Weapon otherWeapon = col.GetComponent<Weapon>();
        if (otherWeapon != null && hitWith == null)
        {
            hitWith = otherWeapon;
            ShakeCamera();
            Invoke("ProcessHit", hitReactionTime);
        }
        */
        /*
        Projectile p = col.GetComponent<Projectile>();
        if (p != null)
        {
            if (p.PlayerNumber == playerNumber)
            {
                if (!p.Moving && !weapon.InHand)
                {
                    weapon.PickUp();
                    Destroy(p.gameObject);
                }
            }
            else if (p.Moving)
            {
                //Score.instance.AddScore(p.PlayerNumber);
                Score.instance.CmdScore(p.PlayerNumber);
                Kill(p.transform.right * deathForce);
            }
        }
        */
    }

    /// <summary>
    /// Triggers the process of getting hit with a weapon
    /// </summary>
    /// <param name="otherWeapon">The weapon that hit us</param>
    /// <param name="processInstantly">Whether the hit should kill instantly or not</param>
    public void GetHit(Weapon otherWeapon, bool processInstantly = false)
    {
        if (!invincible && otherWeapon != null && hitWith == null && PlayerNumber != otherWeapon.PlayerNumber)
        {
            Player otherPlayer = otherWeapon.OurPlayer;
            if (otherPlayer.hitWith != null && otherPlayer.hitWith.PlayerNumber == PlayerNumber)
            {
                Klang(otherWeapon);
                return;
            }
            hitWith = otherWeapon;
            FireEvent(new HitEvent {Attacker = otherPlayer, Hit = this});

            if (processInstantly)
            {
                ProcessHit();
            }
            else
            {
                Invoke("ProcessHit", hitReactionTime);
            }
        }
    }

    /// <summary>
    /// Triggers the process of getting hit with a projectile
    /// </summary>
    /// <param name="p"></param>
    public void GetHit(Projectile p)
    {
        if (!invincible && p != null && p != hitWithProjectile)
        {
            if (!p.Moving && !weapon.InHand)
            {
                weapon.PickUp();
                Destroy(p.gameObject);
            }
            else if (p.Moving && p.PlayerNumber != playerNumber)
            {
                //Score.instance.AddScore(p.PlayerNumber);
                hitWithProjectile = p;
                Score.instance.CmdScore(p.PlayerNumber);
                FireEvent(new KillEvent {Killer = p.OurPlayer, Victim = this, MurderProjectile = p});
                Kill(p.transform.right);
            }
        }
    }

    /// <summary>
    /// Triggers the process of being stunned
    /// </summary>
    public void GetStunned(float stunTime)
    {
        stunned = true;
        ChangeMovementState(GetComponent<InStun>());
        Invoke("EndStun", stunTime);
    }

    private void EndStun()
    {
        stunned = false;
    }

    void ProcessHit()
    {
        if (hitWith == null) return;

        Score s = Score.instance;//FindObjectOfType<Score>();
        Player other = hitWith.OurPlayer;
        if (other == null) Debug.LogError("Get Hit other object is null");

        int otherNum = other.PlayerNumber;
        print(otherNum);

        //s.AddScore(otherNum);
        other.CmdScore();


        //FindObjectOfType<Score>().AddScore(hitWith.GetComponentInParent<Player>().PlayerNumber);
        FireEvent(new KillEvent {Killer = other, Victim = this, MurderWeapon = hitWith});
        Kill(hitWith.transform.right);
    }

    public void CancelHit()
    {
        hitWith = null;
        CancelInvoke("ProcessHit");
    }

    protected void Klang(Weapon other)
    {
        FireEvent(new KlangEvent
        {
            A = weapon,
            B = other
        });
    } 

    #endregion

    #region Helpers
    public void ApplyForce(Vector3 force)
    {
        if (CurrentMovementState is InAttack)
        {
            ChangeMovementState(GetComponent<InAir>());
        }
        OnVelocityChanged(force);
    }

    /// <summary>
    /// Gets the sprite pivot as a -1 to 1 value, where 0 is the middle of the sprite
    /// </summary>
    /// <returns></returns>
    private float SpriteXOffset()
    {
        return (currentSprite.sprite.pivot.x / currentSprite.sprite.rect.width)*2 - 1;
    }

    private void ShakeCamera()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(10f, 10f, 0f, 0.5f);
    }

    [Command]
    private void CmdScore()
    {
        if (!isServer) return;
        //FindObjectOfType<Score>().AddScore(playerNumber);
        Score.instance.AddScore(playerNumber);
    }
	
    private void ChangeMovementState(MovementState next)
    {
        Vector3 newVelocity, newExternalForce;
        movementState.OnExit(velocity, externalForce, out newVelocity, out newExternalForce);
        OnVelocityChanged(newVelocity);
        OnExternalForceChanged(newExternalForce);

        next.OnEnter(velocity, externalForce, out newVelocity, out newExternalForce);
        OnVelocityChanged(newVelocity);
        OnExternalForceChanged(newExternalForce);

        FireEvent(new MovementStateChangedEvent {Next = next, Previous = movementState});

        movementState = next;
        CmdUpdateMovementState(movementState.GetType().ToString());
	}

    public void FireEvent<T>(T e) where T : EventArgs
    {
        EventDispatcher.Instance.FireEvent(this, e);
        dispatcher.FireEvent(this, e);
    }
    #endregion

    #region Data Syncronization

    #region SyncVar Hooks

    public void OnDeath(bool deathState)
    {
        localDead = deathState;
        dead = deathState;
    }

    public void OnVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public void OnExternalForce(Vector3 newExternalForce)
    {
        externalForce = newExternalForce;
    }

    #endregion

    #region Command Hooks

    public void OnDeathChanged(bool deathState)
    {
        localDead = deathState;
        CmdDeathChange(deathState);
    }

    public void OnVelocityChanged(Vector3 newVelocity)
    {
        CmdVelocityChange(newVelocity);
        velocity = newVelocity;
    }

    public void OnExternalForceChanged(Vector3 newExternalForce)
    {
        CmdExternalForceChange(newExternalForce);
    }

    public void OnInvicibleChanged(bool invincible)
    {
        CmdInvincibilityChange(invincible);
    }

    #endregion

    #region Commands

    [Command]
    public void CmdDeathChange(bool deathState)
    {
        localDead = deathState;
        dead = deathState;
    }

    [Command]
    public void CmdVelocityChange(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    [Command]
    public void CmdExternalForceChange(Vector3 newExternalForce)
    {
        externalForce = newExternalForce;
    }

    [Command]
    public void CmdInvincibilityChange(bool invincible)
    {
        this.invincible = invincible;
    }

    #endregion

    [Command]
    private void CmdUpdateMovementState(string state)
    {
        //print("state is '" + state + "'");
        RpcUpdateMovementState(state);
    }

    [ClientRpc]
    private void RpcUpdateMovementState(string state)
        {
        movementState = (MovementState)GetComponent(state);
        }

    [Command]
    private void CmdAttack()
    {
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        weapon.Attack();
    }

    [Command]
    private void CmdSpecialAttack()
    {
        RpcSpecialAttack();
    }

    [ClientRpc]
    private void RpcSpecialAttack()
    {
        weapon.Special();
    }
    #endregion

    #region Main Loop
    //allows inherited classes to interfere with default FSM transitions
    //  by intercepting the desired next state before it reaches
    //  the end user
    //  also lets user do polymorphic events for transitions, if need be.
    //  (note: only gives player scope)
    [Client]
    protected void ChooseNextState(ref MovementState next)
    {
        //if(inputs.attack.Down && weapon.CanAttack && !(movementState is InAttack))
        //if(InputPlayer.GetButtonDown("Attack") && weapon.CanAttack && !(movementState is InAttack) && !movementSpecial.isInUse)
        if (stunned && !(movementState is InStun))
        {
            next = GetComponent<InStun>();
        }
        else if((weapon.IsAttacking || weapon.IsSpecialAttacking) && !(movementState is InAttack || movementState is OnMovementSpecial))
        {
            next = GetComponent<InAttack>();
        }
        //else if (inputs.block.Down && weapon.InHand && shield.CanActivate && !(movementState is InBlock || movementState is InAttack))
        else if (InputPlayer.GetButtonDown("Block") && weapon.InHand && shield.CanActivate && 
            !(movementState is InBlock || movementState is InAttack || movementState is OnMovementSpecial || movementState is InStun))
        {
            next = GetComponent<InBlock>();
        }   

        //else if(inputs.movementSpecial.Down && !(movementState is OnDash) && currentDashes > 0)
        else if(InputPlayer.GetButtonDown("Movement Special")
            && !(InputPlayer.GetButtonDown("Attack") || InputPlayer.GetButtonDown("Weapon Special"))
            && !(weapon.IsAttacking || weapon.IsSpecialAttacking)
            && !(movementState is OnMovementSpecial) && !movementSpecial.isDisabled && movementSpecial.canUse)
        {
            next = GetComponentInChildren<OnMovementSpecial>();
        }

        else if (InputPlayer.GetButtonDown("Taunt") && (movementState is OnGround))
            {
            next = GetComponent<TauntState>();
        }
    }

    [ClientCallback]
    void Update()
    {
        if (!isLocalPlayer)
        {
            //controller.Move(velocity * Time.deltaTime);
            UpdateDirection();
            UpdateHitbox();
            UpdateSprite();
            return;
        }

        //inputs.UpdateInputs();
        OnVelocityChanged(movementState.ApplyFriction(velocity));
        OnExternalForceChanged(movementState.DecayExternalForces(externalForce));

        Vector3 newVelocity;
        Vector3 newExternalForce;

        movementState.ApplyInputs(velocity, externalForce, out newVelocity, out newExternalForce);
        OnVelocityChanged(newVelocity);
        OnExternalForceChanged(newExternalForce);

        controller.Move(velocity * Time.deltaTime + externalForce * Time.deltaTime);

        MovementState next = movementState.DecideNextState(velocity, externalForce);

        if (InputPlayer.GetButtonDown("Attack") && weapon.CanAttack && movementState.AttackAllowed)
        {
            CmdAttack();
        }
        else if (InputPlayer.GetButtonDown("Weapon Special") && weapon.CanSpecialAttack && movementState.AttackAllowed)
        {
            CmdSpecialAttack();
        }

        ChooseNextState(ref next);

        if (next != null)
        {
            ChangeMovementState(next);
        }

        //controller.UpdateBounds(currentSprite.bounds);

        //handle blocking/parrying
        if (hitWith != null)
        {
            if (weapon.InHand && shield.TakeHit(hitWith))
            {
                FireEvent(new BlockEvent {Attacker = hitWith.OurPlayer, Blocker = this});
                CancelHit();
            }
            //else if (!weapon.InHand && inputs.parry.Down)
            else if (!weapon.InHand && InputPlayer.GetButtonDown("Parry"))
            {
                /*
                //steal weapon like a badass
                weapon.InHand = true;
                hitWith.InHand = false;
                FireEvent(new ParryEvent { Attacker = hitWith.OurPlayer, Parrier = this });
                CancelHit();
                */
            }
            else
            {
                Player p = hitWith.OurPlayer;
                if (p.hitWith != null && p.hitWith.PlayerNumber == PlayerNumber)
                {
                    Klang(hitWith);
                }
            }
        }

        //update animation states
        if (anim != null)
        {
            float velMag = Mathf.Abs(velocity.x);
            anim.SetFloat("HorizontalSpeed", velMag);
            anim.SetBool("OnGround", movementState is OnGround);
            anim.SetBool("OnDash", movementState is OnMovementSpecial);
            anim.SetBool("OnWall", movementState is OnWall);
            anim.SetBool("InAttack", movementState is InAttack);
            anim.SetBool("InAir", movementState is InAir);
            anim.SetBool("Blocking", movementState is InBlock);
            anim.SetBool("Hit", hitWith != null);
        }
    }

    [ClientCallback]
    void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!manuallyUpdatedDirection)
        {
            UpdateDirection();
        }
        manuallyUpdatedDirection = false;

        UpdateHitbox();
        UpdateSprite();
    }

    private void UpdateDirection()
    {
        float velMag = Mathf.Abs(velocity.x);
            if (velMag > 0.01f)
            { 
                Vector3 localScale = visuals.localScale;
                localScale.x = Mathf.Sign(velocity.x);
                visuals.localScale = localScale;
            }
    }

    public void UpdateDirection(bool right)
    {
        manuallyUpdatedDirection = true;
        Vector3 localScale = visuals.localScale;
        localScale.x = right ? 1f : -1f;
        visuals.localScale = localScale;
    }

    protected void UpdateHitbox()
    {
        hitbox_collider.size = currentSprite.bounds.size;
        Vector2 newOffset = Vector2.zero;
        newOffset.x = Mathf.Sign(visuals.localScale.x)
            * SpriteXOffset()
            * Mathf.Abs(box.bounds.extents.x - currentSprite.bounds.extents.x);

        newOffset.y = currentSprite.bounds.extents.y - box.bounds.extents.y;
        hitbox_collider.offset = newOffset;
    }

    protected void UpdateSprite()
    {
        Vector3 newPos = Vector3.zero;

        newPos.x = Mathf.Sign(visuals.localScale.x)
            * SpriteXOffset()
            * box.bounds.extents.x;
        newPos.y = -box.bounds.extents.y;

        visuals.localPosition = newPos;
    }
    #endregion
}
