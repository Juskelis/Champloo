using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class Player : NetworkBehaviour
{
    #region Variables

    #region State Variables
    [SyncVar]
    private bool dead = false;

    [SerializeField]
    [Range(1, 4)]
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

    #endregion

    #region Customization Variables

    [SerializeField]
    private Color playerColor = Color.white;

    [SerializeField]
    private SpriteRenderer[] coloredSprites;

    [SerializeField]
    private Transform visuals;

    [SerializeField]
    private Transform spawnOnDeath;

    [SerializeField]
    private float bounceForce = 10f;

    [SerializeField]
    private float deathForce = 20f;

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

    [SyncVar]
    private Vector3 velocity = Vector3.zero;
    [SyncVar]
    private Vector3 externalForce = Vector3.zero;

    public float Gravity { get; set; }

    private Weapon hitWith;

    #endregion

    #region Component Variables

    private InputController inputs;
    private BoxCollider2D box;
    private Controller2D controller;
    private Animator anim;

    private Weapon weapon;
    private Shield shield;

    private OnMovementSpecial movementSpecial;

    #endregion

    #endregion

    #region Initialization

    void Awake()
    {
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

        inputs = GetComponent<InputController>();

        ourNetworkID = (int)GetComponent<NetworkIdentity>().netId.Value;
        int ourPlayerControllerID = GetComponent<NetworkIdentity>().playerControllerId;
        //playerNumber = Utility.GetLocalPlayerNumber(ourPlayerControllerID);
        playerNumber = ourNetworkID;
        InputPlayer = Utility.GetNetworkPlayer(ourPlayerControllerID);
        //transform.position = Random.insideUnitCircle;
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);

        dead = false;

        weapon.PickUp();

        //change colors of child sprites
        foreach(SpriteRenderer s in coloredSprites)
        {
            s.color = playerColor;
        }
        //inputs.playerNumber = playerNumber;

        //attach to events
        controller.Crushed += Crushed;
        controller.Collision += Collided;

        //add to score register
        Score.AddPlayer(ourNetworkID);
    }

    #endregion

    #region De-Initialization

    void Destroy()
    {
        //detach from events
        controller.Crushed -= Crushed;
        controller.Collision -= Collided;

        //detach from score register
        Score.RemovePlayer(ourNetworkID);
    }
        
    #endregion
        
    #region Spawning

    private void Spawn()
    {
        gameObject.SetActive(true);
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);
        dead = false;
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

        velocity = Vector3.zero;

        velocity = Vector3.zero;
        externalForce = Vector3.zero;
        hitWith = null;

        if (spawnOnDeath != null)
        {
            Transform corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
            Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
            corpseBody.gravityScale = Gravity / Physics2D.gravity.magnitude;
            corpseBody.velocity = direction;
        }

        gameObject.SetActive(false);
        dead = true;
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

        Weapon otherWeapon = col.GetComponent<Weapon>();
        if (otherWeapon != null && hitWith == null)
        {
            hitWith = otherWeapon;
            ShakeCamera();
            Invoke("GetHit", hitReactionTime);
        }
    }

    [ClientCallback]
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.activeSelf)
        {
            return;
        }

        Projectile p = col.gameObject.GetComponent<Projectile>();
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
    }

    void GetHit()
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
        if (Mathf.Abs(force.x) > threshold) velocity.x = force.x;
        if (Mathf.Abs(force.y) > threshold) velocity.y = force.y;
        if (Mathf.Abs(force.z) > threshold) velocity.z = force.z;
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
        if(InputPlayer.GetButtonDown("Attack") && weapon.CanAttack && !(movementState is InAttack))
        {
            //weapon.Attack();
            CmdAttack();
            next = GetComponent<InAttack>();
        }
        //else if (inputs.block.Down && weapon.InHand && shield.CanActivate && !(movementState is InBlock || movementState is InAttack))
        else if (InputPlayer.GetButtonDown("Block") && weapon.InHand && shield.CanActivate && !(movementState is InBlock || movementState is InAttack))
        {
            next = GetComponent<InBlock>();
        }

        //else if(inputs.movementSpecial.Down && !(movementState is OnDash) && currentDashes > 0)
        else if(InputPlayer.GetButtonDown("Movement Special") && !(movementState is OnDash) && currentDashes > 0)
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
        
        MovementState next = movementState.UpdateState(ref velocity, ref externalForce);

        ChooseNextState(ref next);

        //HandleCollisions();

        if (next != null)
        {
            movementState.OnExit(ref velocity, ref externalForce);
            next.OnEnter(ref velocity, ref externalForce);
            movementState = next;
            CmdUpdateMovementState(movementState.GetType().ToString());
        }

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

        //if (inputs.weaponSpecial.Down)
        if (InputPlayer.GetButtonDown("Weapon Special"))
        {
            weapon.Special();
        }

        //update animation states
        if (anim != null)
        {
            UpdateDirection();
            float velMag = Mathf.Abs(velocity.x);
            anim.SetFloat("HorizontalSpeed", velMag);
            anim.SetBool("OnGround", movementState is OnGround);
            //anim.SetBool("OnDash", movementState is OnDash);
            anim.SetBool("OnWall", movementState is OnWall);
            //anim.SetBool("InAttack", movementState is InAttack);
            anim.SetBool("InAir", movementState is InAir);
            anim.SetBool("Hit", hitWith != null);
        }
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
    #endregion
}
