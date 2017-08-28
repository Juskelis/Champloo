using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyCrusher : MonoBehaviour {

    [SerializeField]
    private LayerMask crushMask;

    [SerializeField]
    private Transform spawnOnCrush;

    void Update()
    {
        if (Physics2D.OverlapPoint(transform.position, crushMask))
        {
            Crush();
        }
    }

    private void Crush()
    {
        Instantiate(spawnOnCrush, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
