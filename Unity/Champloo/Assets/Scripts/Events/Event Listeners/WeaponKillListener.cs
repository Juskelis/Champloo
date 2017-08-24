using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKillListener : MonoBehaviour
{
    [SerializeField]
    private Transform effect;

    void Start()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<KillEvent>(OnKill);
    }

    private void OnKill(object sender, EventArgs args)
    {
        KillEvent e = (KillEvent)args;

        Vector3 dirToKiller = (e.Killer.transform.position - e.Victim.transform.position).normalized;
        Instantiate(
            effect,
            e.Victim.transform.position,
            Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(dirToKiller),
                    transform.forward));
    }
}
