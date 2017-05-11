using System;
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

    //[SerializeField]
    //[Range(1, 4)]
    private int playerNumber = 1;
    public int PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

    private int ourNetworkID;

    private int currentDashes = 0;
    
    private MovementState movementState;
    public MovementState CurrentMovementState
    {
        get { return movementState; }
    }

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
                aim.x = HorizontalDirection;
                aim.y = 0;
            }
            return aim;
        }
    }

    #endregion

    #region Customization Variables

    [SerializeField]
    private Color playerColor = Color.white;

    [SerializeField]
    private SpriteRenderer[] coloredSprites;

    [SerializeField]
    private Transform visuals;

    [SerializeField]
    private Transform hitbox;

    [SerializeField]
    private Transform spawnOnDeath;

    [SerializeField]
    private float bounceForce = 10f;

    [SerializeField]
    private float deathForce = 20f;

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

    #endregion

    #region Component Variables

    private InputController inputs;
    private BoxCollider2D box;
    private BoxCollider2D hitbox_collider;
    private Controller2D controller;
    private Animator anim;

    private Weapon weapon;
    private Shield shield;

    private OnMovementSpecial movementSpecial;

    private SpriteRenderer currentSprite;
    #endregion

    #endregion

    #region Initialization

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
        gameObject.SetActive(true);
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);
        //dead = false;
        OnDeathChanged(false);
        //velocity = Vector3.zero;
        //externalForce = Vector3.zero;
        OnVelocityChanged(Vector3.zero);
        OnExternalForceChanged(Vector3.zero);
        Vector3 garbageVelocity;
        Vector3 garbageExternalForces;
        movementState.OnExit(velocity, externalForce, out garbageVelocity, out garbageExternalForces);
        movementState = GetComponent<OnGround>();
        movementState.OnEnter(velocity, externalForce, out garbageVelocity, out garbageExternalForces);
        CmdUpdateMovementState(movementState.GetType().ToString());
        weapon.PickUp();
    }

    #endregion

    #region Death

    [Command]
    void CmdKill(Vector3 direction)
    {
        RpcKilled(direction);
    }

    //[Client]
    void Kill(Vector3 direction = default(Vector3))
    {
        if (isLocalPlayer && hasAuthority)
    {
            CmdKill(direction);
        }
        /*
        else if (isServer)
        {
            RpcKilled(direction);
        }
        else
        {
            //local copy of other player got killed
            //  so we need to notify authoritative version
            //  of other player that they died
        }
        */
    }

    [ClientRpc]
    void RpcKilled(Vector3 direction)
    {
        ShakeCamera();

        //velocity = Vector3.zero;
        //externalForce = Vector3.zero;
        OnVelocityChanged(Vector3.zero);
        OnExternalForceChanged(Vector3.zero);

        hitWith = null;

        if (spawnOnDeath != null)
        {
            Transform corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
            Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
            corpseBody.gravityScale = Gravity / Physics2D.gravity.magnitude;
            corpseBody.velocity = direction;
        }

        gameObject.SetActive(false);
        //dead = true;
        OnDeathChanged(true);
        float time = FindObjectOfType<PlayerSpawner>().SpawnTime;
        Invoke("Spawn", time);
    }

    #endregion

    #region Collision Callbacks
    [Client]
    void Crushed(object sender, GameObject obj)
    {
        
        if (obj.GetComponent<Player>() != null) return;
        FindObjectOfType<Score>().SubtractScore(playerNumber);
        Kill();
        
    }

    void Collided(object sender, GameObject other, Controller2D.CollisionInfo info)
    {
        Player otherPlayer = other.GetComponent<Player>();
        if (otherPlayer == null) return;
        
        //This section deals with player collisions
        //This section needs to be changed to not reference dash
        //If another player is below 
        if (info.Below)
        {
            //bounce off their head no matter what
            ApplyForce(Vector3.up * bounceForce);
            if (movementState is OnDash && !otherPlayer.dead)
            {
                //kill them!
                CmdScore();
                otherPlayer.Kill(Vector3.down);
            }
        }
        //if another player is above
        else if (info.Above)
        {
            if (otherPlayer.movementState is OnDash && !dead)
            {
                //they killed us!
                Kill(Vector3.down);
                otherPlayer.ApplyForce(Vector3.up * bounceForce);
                otherPlayer.CmdScore();
            }
        }
        //if another player is to the side
        else if (info.Left || info.Right)
        {
            //bounce away
            float dir = Mathf.Sign(transform.position.x - other.transform.position.x);
            ApplyForce(Vector3.right * dir * bounceForce);
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
    public void GetHit(Weapon otherWeapon)
    {
        if (otherWeapon != null && hitWith == null && PlayerNumber != otherWeapon.PlayerNumber)
        {
            hitWith = otherWeapon;
            ShakeCamera();
            Invoke("ProcessHit", hitReactionTime);
        }
    }

    /// <summary>
    /// Triggers the process of getting hit with a projectile
    /// </summary>
    /// <param name="p"></param>
    public void GetHit(Projectile p)
    {
        if (p != null && p != hitWithProjectile)
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
                hitWithProjectile = p;
                Score.instance.CmdScore(p.PlayerNumber);
                Kill(p.transform.right * deathForce);
            }
        }
    }

    /// <summary>
    /// Triggers the process of being stunned
    /// </summary>
    public void GetStunned()
    {
        
    }

    void ProcessHit()
    {
        if (hitWith == null) return;

        Score s = Score.instance;//FindObjectOfType<Score>();
        Player other = hitWith.GetComponentInParent<Player>();
        if (other == null) Debug.LogError("Get Hit other object is null");

        int otherNum = other.PlayerNumber;
        print(otherNum);

        //s.AddScore(otherNum);
        other.CmdScore();


        //FindObjectOfType<Score>().AddScore(hitWith.GetComponentInParent<Player>().PlayerNumber);
        Kill(hitWith.transform.right * deathForce);
    }

    #endregion

    #region Helpers
    public void ApplyForce(Vector3 force)
    {
        //velocity = force;
        float threshold = 0.25f;
        Vector3 newVelocity = velocity;
        if (Mathf.Abs(force.x) > threshold) newVelocity.x = force.x;
        if (Mathf.Abs(force.y) > threshold) newVelocity.y = force.y;
        if (Mathf.Abs(force.z) > threshold) newVelocity.z = force.z;
        OnVelocityChanged(newVelocity);
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
    #endregion

    #region Data Syncronization

    #region SyncVar Hooks

    public void OnDeath(bool deathState)
    {
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
        CmdDeathChange(deathState);
    }

    public void OnVelocityChanged(Vector3 newVelocity)
    {
        CmdVelocityChange(newVelocity);
    }

    public void OnExternalForceChanged(Vector3 newExternalForce)
    {
        CmdExternalForceChange(newExternalForce);
    }

    #endregion

    #region Commands

    [Command]
    public void CmdDeathChange(bool deathState)
    {
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
        if(weapon.AttackState == TimingState.IN_PROGRESS && !(movementState is InAttack))
        {
            next = GetComponent<InAttack>();
        }
        //else if (inputs.block.Down && weapon.InHand && shield.CanActivate && !(movementState is InBlock || movementState is InAttack))
        else if (InputPlayer.GetButtonDown("Block") && weapon.InHand && shield.CanActivate && !(movementState is InBlock || movementState is InAttack))
        {
            next = GetComponent<InBlock>();
        }

        //else if(inputs.movementSpecial.Down && !(movementState is OnDash) && currentDashes > 0)
        else if(InputPlayer.GetButtonDown("Movement Special") && !(movementState is OnMovementSpecial) && !movementSpecial.isDisabled)
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

        ChooseNextState(ref next);

        if (InputPlayer.GetButtonDown("Attack") && weapon.CanAttack && !movementSpecial.isInUse)
        {
            CmdAttack();
        }
        else if (InputPlayer.GetButtonDown("Weapon Special"))
        {
            CmdSpecialAttack();
        }

        if (next != null)
        {
            movementState.OnExit(velocity, externalForce, out newVelocity, out newExternalForce);
            OnVelocityChanged(newVelocity);
            OnExternalForceChanged(newExternalForce);

            next.OnEnter(velocity, externalForce, out newVelocity, out newExternalForce);
            OnVelocityChanged(newVelocity);
            OnExternalForceChanged(newExternalForce);

            movementState = next;
            CmdUpdateMovementState(movementState.GetType().ToString());
        }

        //controller.UpdateBounds(currentSprite.bounds);

        //handle blocking/parrying
        if (hitWith != null)
        {
            if (!hitWith.isActiveAndEnabled || (weapon.InHand && shield.TakeHit()))
            {
                hitWith = null;
            }
            //else if (!weapon.InHand && inputs.parry.Down)
            else if (!weapon.InHand && InputPlayer.GetButtonDown("Parry"))
            {
                //steal weapon like a badass
                weapon.InHand = true;
                hitWith.InHand = false;
                hitWith = null;
            }
        }

        //update animation states
        if (anim != null)
        {
            float velMag = Mathf.Abs(velocity.x);
            anim.SetFloat("HorizontalSpeed", velMag);
            anim.SetBool("OnGround", movementState is OnGround);
            anim.SetBool("OnDash", movementState is OnDash);
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

        UpdateDirection();
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
            * (box.bounds.extents.x - currentSprite.sprite.bounds.extents.x);

        newOffset.y = currentSprite.sprite.bounds.extents.y - box.bounds.extents.y;
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
