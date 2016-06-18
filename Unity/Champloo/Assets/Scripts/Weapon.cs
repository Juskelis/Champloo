using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {
    public bool InHand { get; set; }

    public bool CanAttack { get { return InHand && attackTimer <= 0 && reloadTimer <= 0; } }

    public bool IsAttacking { get { return attackTimer <= 0; } }

    [SerializeField]
    private float attackTime;
    [SerializeField]
    private float reloadTime;

    [SerializeField]
    private Projectile thrownVersion;

    private float attackTimer;
    private float reloadTimer;

    private InputController input;

    private SpriteRenderer renderer;
    private BoxCollider2D collider2D;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<BoxCollider2D>();

        InHand = true;
        input = GetComponentInParent<InputController>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.AngleAxis(input.leftStickAngle, Vector3.forward);

        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else if(reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        StartAttack();
        Invoke("EndAttack", attackTime);
    }

    private void StartAttack()
    {
        if (!InHand || attackTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        attackTimer = attackTime;
        reloadTimer = reloadTime;

        renderer.enabled = true;
        collider2D.enabled = true;
    }

    private void EndAttack()
    {
        renderer.enabled = false;
        collider2D.enabled = false;
    }

    public void Throw()
    {
        if (InHand)
        {
            InHand = false;
            Instantiate(thrownVersion, transform.position, transform.rotation);
        }
    }

    public void PickUp()
    {
        if (!InHand)
        {
            InHand = true;
        }
    }
}
