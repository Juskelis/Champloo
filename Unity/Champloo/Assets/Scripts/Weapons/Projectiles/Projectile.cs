using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public int PlayerNumber { get; set; }

    [SerializeField] protected LayerMask obstacleMask;

    [SerializeField] protected float speed = 1f;

    private bool moving = true;
    public bool Moving { get { return moving; } }

    private Transform follow;
    private Vector3 relativePos;

    protected Rigidbody2D body;

    protected Player player;

    protected static List<Player> players;
    protected static int destroyableProjectiles;

    private bool destroyable_doNotModifyDirectly = false;
    protected bool CanBeDestroyed
    {
        get
        {
            return destroyable_doNotModifyDirectly;
        }
        set
        {
            if (destroyable_doNotModifyDirectly != value)
            {
                destroyableProjectiles += value ? 1 : -1;
            }
            destroyable_doNotModifyDirectly = value;
        }
    }

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if(players == null) players = new List<Player>(FindObjectsOfType<Player>());
    }

    protected virtual void Start()
    {
        //check collisions
        foreach (var col in Physics2D.OverlapBoxAll(transform.position, transform.localScale, transform.rotation.eulerAngles.z, obstacleMask))
        {
            OnTriggerEnter2D(col);
        }
        player = FindPlayer();
        if (player == null)
        {
            Debug.LogError("Could not find player!");
        }
    }

    protected virtual void Update()
    {
        body.velocity = Vector2.zero;
        if (moving)
        {
            //transform.Translate(Vector2.right*speed*Time.deltaTime);
            body.velocity = transform.TransformVector(Vector2.right*speed);
        }
        else {
            if (follow != null)
            {
                transform.position = follow.position - relativePos;
            }
            //delete if stopped, and too many still ones on screen
            if (CanBeDestroyed && player.Dead
                && players.Count - DeadPlayerCount() < destroyableProjectiles)
            {
                Destroy(gameObject);
            }
        }
    }

    private int DeadPlayerCount()
    {
        int ret = 0;
        foreach (var p in players)
        {
            if (p.Dead) ret++;
        }
        return ret;
    }

    private Player FindPlayer()
    {
        foreach (var p in players)
        {
            if (p.PlayerNumber == PlayerNumber)
            {
                return p;
            }
        }
        return null;
    }

    protected virtual void OnDestroy()
    {
        if (!Moving) destroyableProjectiles--;
    }

    protected virtual void OnTriggerEnter2D(Collider2D c)
    {
        ProcessHitPlayer(c.gameObject);

        //check whether we need to stop
        if (moving && (obstacleMask.value & (1 << c.gameObject.layer)) > 0)
        {
            ProcessHitObstacle(c);
        }
    }

    protected virtual void ProcessHitPlayer(GameObject g)
    {
        Player p = g.gameObject.GetComponent<Player>()
                   ?? g.gameObject.GetComponentInParent<Player>();
        if (p == null) return;
        p.GetHit(this);
    }
    
    protected virtual void ProcessHitObstacle(Collider2D c)
    {
        follow = c.transform;
        relativePos = follow.position - transform.position;
        moving = false;
        CanBeDestroyed = true;

        EZCameraShake.CameraShaker.Instance.ShakeOnce(5, 5, 0, 0.5f);

        GetComponent<TrailRenderer>().enabled = false;
    }
}
