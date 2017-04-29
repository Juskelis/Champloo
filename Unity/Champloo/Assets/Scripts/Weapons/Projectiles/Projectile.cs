using UnityEngine;
using System.Collections;
using System.Linq;
using TypeReferences;

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

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        body.velocity = Vector2.zero;
        if (moving)
        {
            //transform.Translate(Vector2.right*speed*Time.deltaTime);
            body.velocity = transform.TransformVector(Vector2.right*speed);
        }
        else if(follow != null)
        {
            transform.position = follow.position - relativePos;
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
    }
    
    protected virtual void ProcessHitObstacle(Collider2D c)
    {
        follow = c.transform;
        relativePos = follow.position - transform.position;
        moving = false;

        EZCameraShake.CameraShaker.Instance.ShakeOnce(5, 5, 0, 0.5f);

        GetComponent<TrailRenderer>().enabled = false;
    }
}
