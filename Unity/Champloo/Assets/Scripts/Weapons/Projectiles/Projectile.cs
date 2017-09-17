using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public int PlayerNumber { get; set; }

    [SerializeField] protected LayerMask obstacleMask;

    [SerializeField] protected AnimationCurve velocityOverTime;

    [SerializeField] protected float minVelocity = 0f;
    [SerializeField] protected float maxVelocity = 1f;

    [SerializeField] protected float velocityAnimationTime = 1f;

    protected float Speed
    {
        get { return minVelocity + velocityOverTime.Evaluate((Time.time - startTime)/velocityAnimationTime)*(maxVelocity - minVelocity); }
    }

    protected bool moving = true;
    public bool Moving { get { return moving; } }

    private Transform follow;
    private Vector3 relativePos;

    private float startTime;

    protected Rigidbody2D body;

    protected Player player;
    public Player OurPlayer { get {return player;} }

    private bool destroyable_doNotModifyDirectly = false;
    protected bool CanBeDestroyed
    {
        get
        {
            return destroyable_doNotModifyDirectly;
        }
        set
        {
            destroyable_doNotModifyDirectly = value;
        }
    }

    protected Collider2D hitbox;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        startTime = Time.time;
        //check collisions
        foreach (var col in Physics2D.OverlapBoxAll(transform.position, hitbox.bounds.size, transform.rotation.eulerAngles.z, obstacleMask))
        {
            OnTriggerEnter2D(col);
        }
        player = FindPlayer();
        if (player == null)
        {
            Debug.LogError("Could not find player with PlayerNumber " + PlayerNumber);
        }
    }

    protected virtual void Update()
    {
        body.velocity = Vector2.zero;
        if (moving)
        {
            //transform.Translate(Vector2.right*speed*Time.deltaTime);
            body.velocity = transform.TransformVector(Vector2.right*Speed);
        }
        else {
            if (follow != null)
            {
                transform.position = follow.position - relativePos;
            }
            //delete if stopped, and too many still ones on screen
            if (CanBeDestroyed && CharactersWithoutWeapon() < CullableProjectiles())
            {
                CanBeDestroyed = false;
                Destroy(gameObject);
            }
        }
    }

    private int CharactersWithoutWeapon()
    {
        int ret = 0;
        foreach (Weapon weapon in FindObjectsOfType<Weapon>())
        {
            if (!weapon.InHand)
            {
                ret++;
            }
        }
        return ret;
    }

    private int CullableProjectiles()
    {
        int ret = 0;
        foreach (Projectile projectile in FindObjectsOfType<Projectile>())
        {
            if (projectile.CanBeDestroyed) ret++;
        }
        return ret;
    }

    private Player FindPlayer()
    {
        foreach (var p in FindObjectsOfType<Player>())
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
        if (CanBeDestroyed)
        {
            CanBeDestroyed = false;
        }
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
        EventDispatcher.Instance.FireEvent(this, new ProjectileHitEvent { HitPlayer = p, HitGameObject = g });
    }
    
    protected virtual void ProcessHitObstacle(Collider2D c)
    {
        EventDispatcher.Instance.FireEvent(this, new ProjectileHitEvent { HitPlayer = null, HitGameObject = c.gameObject });

        transform.position = PointOnBoundingBox(c.bounds, transform.position, transform.TransformVector(Vector2.right));

        follow = c.transform;
        relativePos = follow.position - transform.position;
        moving = false;
        CanBeDestroyed = true;

        EZCameraShake.CameraShaker.Instance.ShakeOnce(5, 5, 0, 0.5f);

        GetComponent<TrailRenderer>().enabled = false;
    }

    private Vector3 PointOnBoundingBox(Bounds b, Vector3 point, Vector3 forward)
    {
        if (b.Contains(point))
        {
            Vector3 topRight = b.max;
            Vector3 topLeft = new Vector3(b.min.x, b.max.y);
            Vector3 bottomRight = new Vector3(b.max.x, b.min.y);
            Vector3 bottomLeft = b.min;

            Vector2 intersectionPoint = Vector2.zero;
            if (Utility.RayLineSegmentIntersection2D(point, -forward, topRight, topLeft, ref intersectionPoint))
            {
                return intersectionPoint;
            }
            if (Utility.RayLineSegmentIntersection2D(point, -forward, topLeft, bottomLeft, ref intersectionPoint))
            {
                return intersectionPoint;
            }
            if (Utility.RayLineSegmentIntersection2D(point, -forward, bottomRight, bottomLeft, ref intersectionPoint))
            {
                return intersectionPoint;
            }
            if (Utility.RayLineSegmentIntersection2D(point, -forward, topRight, bottomRight, ref intersectionPoint))
            {
                return intersectionPoint;
            }
            Debug.LogError("Unable to find appropriate edge");
            return point;
        }
        else
        {
            return b.ClosestPoint(point);
        }
    }
}
