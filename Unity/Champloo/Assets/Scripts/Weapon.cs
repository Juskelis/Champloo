using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public bool InHand { get; set; }

    public bool CanAttack { get { return InHand && attackTimer <= 0 && reloadTimer <= 0; } }

    public bool IsAttacking { get { return attackTimer <= 0; } }

    [SerializeField]
    private float attackTime;
    [SerializeField]
    private float reloadTime;

    private float attackTimer;
    private float reloadTimer;

    private void Start()
    {
        InHand = true;
    }
    
    private void Update()
    {
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
        if(!InHand || attackTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        attackTimer = attackTime;
        reloadTimer = reloadTime;
    }
}
