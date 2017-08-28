using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKillListener : MonoBehaviour
{
    [SerializeField]
    private Transform killEffect;

    private Vector3 dirToAttacker;
    private int killingWeaponPlayerId;

    void Start()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        dispatcher.AddListener<KillEvent>(OnKill);
        dispatcher.AddListener<HitEvent>(OnHit);
    }

    private void OnHit(object sender, EventArgs args)
    {
        HitEvent hit = (HitEvent)args;


        dirToAttacker = (hit.Attacker.transform.position - hit.Hit.transform.position).normalized;
        killingWeaponPlayerId = hit.Attacker.PlayerNumber;
    }

    private void OnKill(object sender, EventArgs args)
    {
        KillEvent e = (KillEvent)args;

        //don't spawn unless its a weapon kill
        if (e.MurderWeapon == null) return;

        if (e.MurderWeapon.PlayerNumber != killingWeaponPlayerId)
        {
            killingWeaponPlayerId = -1;
            Vector3 dirToKiller = (e.Killer.transform.position - e.Victim.transform.position).normalized;
            Instantiate(
                killEffect,
                e.Victim.transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(dirToKiller),
                    transform.forward));
        }
        else
        {
            Instantiate(
                killEffect,
                e.Victim.transform.position,
                Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(dirToAttacker),
                    transform.forward));
        }
    }
}
