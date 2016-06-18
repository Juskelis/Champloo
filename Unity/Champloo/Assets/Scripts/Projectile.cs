using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    [SerializeField] private LayerMask collisionMask;

    [SerializeField] private float speed = 1f;

    private bool moving = true;

    void Update()
    {
        if (moving)
        {
            transform.Translate(Vector2.right*speed*Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!moving) return;
        if ((collisionMask.value & (1 << c.gameObject.layer)) > 0)
        {
            //transform.parent = c.transform;
            moving = false;
        }
    }
}
