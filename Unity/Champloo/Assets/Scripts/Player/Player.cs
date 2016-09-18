using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class Player : NetworkBehaviour
{
    private bool dead = false;

    [SerializeField]
    [Range(1,4)]
    private int playerNumber = 1;

    [SerializeField]
    private Color playerColor = Color.white;
    [SerializeField]
    private SpriteRenderer[] coloredSprites;
    [SerializeField]
    private Transform visuals;

    public int PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

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

    [SerializeField] private Transform spawnOnDeath;

    private InputController inputs;
    private BoxCollider2D box;
    private Controller2D controller;
    private Animator anim;
    
    [SerializeField]
    private Transform weaponPrefab;
    [SerializeField]
    private Transform shieldPrefab;

    private Weapon weapon;
    private Shield shield;

    [SerializeField]
    private float bounceForce = 10f;
    [SerializeField]
    private float deathForce = 20f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 externalForce = Vector3.zero;

    private int currentDashes = 0;
    
    public float Gravity { get; set; }

    public float hitReactionTime;

    private MovementState movementState;
    public MovementState CurrentMovementState
    {
        get { return movementState; }
    }

    private Weapon hitWith;

    void Awake()
    {
        movementState = GetComponent<OnGround>();
        box = GetComponent<BoxCollider2D>();
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();

        inputs = GetComponent<InputController>();

        int ourNetworkID = GetComponent<NetworkIdentity>().playerControllerId;
        playerNumber = Utility.GetLocalPlayerNumber(ourNetworkID);
        InputPlayer = Utility.GetNetworkPlayer(ourNetworkID);

        //transform.position = Random.insideUnitCircle;
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);
    }

    public void Start ()
    {
        weapon = GetComponentInChildren<Weapon>();
        shield = GetComponentInChildren<Shield>();

        dead = false;

        weapon.PickUp();

        //change colors of child sprites
        foreach(SpriteRenderer s in coloredSprites)
        {
            s.color = playerColor;
        }
        //inputs.playerNumber = playerNumber;

        currentDashes = GetComponent<OnDash>().DashLimit;

        //attach to events
        controller.Crushed += Crushed;
        controller.Collision += Collided;
    }

    private void Spawn()
    {
        gameObject.SetActive(true);
        transform.position = FindObjectOfType<PlayerSpawner>().FindValidSpawn(this);
        dead = false;
        weapon.PickUp();
    }

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
        
        //at this point, we are only dealing with player collisions
        if (info.Below)
        {
            //bounce off their head no matter what
            ApplyForce(Vector3.up * bounceForce);
            if (movementState is OnDash && !otherPlayer.dead)
            {
                //kill them!
                Score();
                otherPlayer.Kill(Vector3.down);
            }
        }
        else if (info.Above)
        {
            if (otherPlayer.movementState is OnDash && !dead)
            {
                //they killed us!
                Kill(Vector3.down);
                otherPlayer.ApplyForce(Vector3.up * bounceForce);
                otherPlayer.Score();
            }
        }
        else if (info.Left || info.Right)
        {
            //bounce away
            float dir = Mathf.Sign(transform.position.x - other.transform.position.x);
            ApplyForce(Vector3.right * dir * bounceForce);
        }
    }

    public void ApplyForce(Vector3 force)
    {
        //velocity = force;
        float threshold = 0.25f;
        if (Mathf.Abs(force.x) > threshold) velocity.x = force.x;
        if (Mathf.Abs(force.y) > threshold) velocity.y = force.y;
        if (Mathf.Abs(force.z) > threshold) velocity.z = force.z;
    }

    void ShakeCamera()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(10f, 10f, 0f, 0.5f);
    }

    private void Score()
    {
        FindObjectOfType<Score>().AddScore(playerNumber);
    }

    void Kill(Vector3 direction = default(Vector3))
    {
        print("killed");

        ShakeCamera();

        velocity = Vector3.zero;
        externalForce = Vector3.zero;
        hitWith = null;

        if (spawnOnDeath != null)
        {
            Transform corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
            Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
            corpseBody.gravityScale = Gravity/Physics2D.gravity.magnitude;
            corpseBody.velocity = direction;
        }

        gameObject.SetActive(false);
        dead = true;
        float time = FindObjectOfType<PlayerSpawner>().SpawnTime;
        print("Spawn time: " + time);
        Invoke("Spawn", time);
    }

    void Destroy()
    {
        //detach from events
        controller.Crushed -= Crushed;
        controller.Collision -= Collided;
    }

    //allows inherited classes to interfere with default FSM transitions
    //  by intercepting the desired next state before it reaches
    //  the end user
    //also lets user do polymorphic events for transitions, if need be.
    //  (note: only gives player scope)
    protected void ChooseNextState(ref MovementState next)
    {
        //if(inputs.attack.Down && weapon.CanAttack && !(movementState is InAttack))
        if(InputPlayer.GetButtonDown("Attack") && weapon.CanAttack && !(movementState is InAttack))
        {
            weapon.Attack();
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
            currentDashes--;
            TrailRenderer tail = GetComponent<TrailRenderer>();
            tail.enabled = true;
            tail.Clear();
            next = GetComponent<OnDash>();
            //Vector2 leftStickDir = inputs.leftStick.normalized;
            Vector2 leftStickDir =
                (Vector2.right*InputPlayer.GetAxis("Aim Horizontal") +
                 Vector2.up*InputPlayer.GetAxis("Aim Vertical")).normalized;
            velocity = ((leftStickDir == Vector2.zero)?Vector2.up:leftStickDir) * ((OnDash)next).DashForce;
        }
        //else if(inputs.taunt.Down && (movementState is OnGround))
        else if(InputPlayer.GetButtonDown("Taunt") && (movementState is OnGround))
        {
            next = GetComponent<TauntState>();
        }

        if (next != null)
        {
            if (movementState is OnDash)
            {
                GetComponent<TrailRenderer>().enabled = false;
            }
            if(movementState is OnGround)
            {
                currentDashes = GetComponent<OnDash>().DashLimit;
            }
            if (movementState is OnWall)
            {
                currentDashes = Mathf.Max(currentDashes, 1);
            }
        }
    }

    void Update()
    {
        //inputs.UpdateInputs();
        
        MovementState next = movementState.UpdateState(ref velocity, ref externalForce);

        ChooseNextState(ref next);

        //HandleCollisions();

        if (next != null)
        {
            movementState.OnExit();
            next.OnEnter();
            movementState = next;
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
            float velMag = Mathf.Abs(velocity.x);
            anim.SetFloat("HorizontalSpeed", velMag);
            anim.SetBool("OnGround", movementState is OnGround);
            anim.SetBool("OnDash", movementState is OnDash);
            anim.SetBool("OnWall", movementState is OnWall);
            //anim.SetBool("InAttack", movementState is InAttack);
            anim.SetBool("InAir", movementState is InAir);
            anim.SetBool("Hit", hitWith != null);
            if (velMag > 0.01f)
            { 
                Vector3 localScale = visuals.localScale;
                localScale.x = Mathf.Sign(velocity.x);
                visuals.localScale = localScale;
            }
        }
    }

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.activeSelf)
        {
            return;
        }

        Projectile p = col.gameObject.GetComponent<Projectile>();
        if(p != null)
        {
            if(p.PlayerNumber == playerNumber)
            {
                if (!p.Moving && !weapon.InHand)
                {
                    weapon.PickUp();
                    Destroy(p.gameObject);
                }
            }
            else if(p.Moving)
            {
                FindObjectOfType<Score>().AddScore(p.PlayerNumber);
                Kill(p.transform.right * deathForce);
            }
        }
    }

    void GetHit()
    {
        if (hitWith == null) return;

        Score s = FindObjectOfType<Score>();
        Player other = hitWith.GetComponentInParent<Player>();
        if (other == null) print("oh no the other player is null");

        int otherNum = other.PlayerNumber;
        print(otherNum);

        s.AddScore(otherNum);


        //FindObjectOfType<Score>().AddScore(hitWith.GetComponentInParent<Player>().PlayerNumber);
        Kill(hitWith.transform.right * deathForce);
    }
}
