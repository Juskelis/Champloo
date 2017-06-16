using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingIndicator : MonoBehaviour
{
    private Player p;

    void Awake()
    {
        p = GetComponentInParent<Player>();
    }

    void Update()
    {
        Vector2 aim = p.AimDirection;
        transform.rotation = Quaternion.AngleAxis(
            Utility.Vector2AsAngle(aim),
            transform.forward
        );
    }
}
