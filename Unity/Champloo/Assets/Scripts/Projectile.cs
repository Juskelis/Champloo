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

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!moving) return;
        if ((collisionMask.value & (1 << c.gameObject.layer)) > 0)
        {
            /*
            Vector2 average = Vector2.zero;
            foreach (var contact in c.contacts)
            {
                average += contact.point;
            }
            transform.position = average/c.contacts.Length;
            */
            /*
            Collider2D col = c.collider;
            Vector2 pos = transform.position;
            while (col.OverlapPoint(pos))
            {
                pos.x -= transform.right.x;
                pos.y -= transform.right.y;
            }
            transform.position = pos;
            */
            follow = c.transform;
            relativePos = follow.position - transform.position;
            moving = false;

            GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
