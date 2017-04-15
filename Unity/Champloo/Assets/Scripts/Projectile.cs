using UnityEngine;
using System.Collections;
using System.Linq;

public class Projectile : MonoBehaviour
{
    public int PlayerNumber { get; set; }

    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private float speed = 1f;

    private bool moving = true;
    public bool Moving { get { return moving; } }

    private Transform follow;
    private Vector3 relativePos;

    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
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

    void OnTriggerEnter2D(Collider2D c)
    {
        if (!moving) return;
        if ((collisionMask.value & (1 << c.gameObject.layer)) > 0)
        {
            Player p = c.GetComponent<Player>();
            p = p != null ? p : c.GetComponentInParent<Player>();
            if (p != null && p.PlayerNumber == PlayerNumber) return;
            follow = c.transform;
            relativePos = follow.position - transform.position;
            moving = false;

            EZCameraShake.CameraShaker.Instance.ShakeOnce(5, 5, 0, 0.5f);

            GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
