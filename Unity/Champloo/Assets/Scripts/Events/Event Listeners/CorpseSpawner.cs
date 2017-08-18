﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseSpawner : MonoBehaviour {

    [SerializeField]
    private Transform spawnOnDeath;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

    [SerializeField]
    private float corpsePushFactor = 1f;

    [SerializeField]
    private Transform bloodSpurt;
    [SerializeField]
    private bool alignToDirection = false;

    private Transform corpse;
    private Transform spurt;

    void Start()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        dispatcher.AddListener<DeathEvent>(OnDeath);
        dispatcher.AddListener<KillEvent>(OnKill);
    }

    private void OnKill(object sender, EventArgs args)
    {
        KillEvent e = (KillEvent)args;

        Vector3 dirToKiller = (e.Killer.transform.position - e.Victim.transform.position).normalized;
        spurt = Instantiate(bloodSpurt, e.Victim.transform.position,
            alignToDirection
                ? Quaternion.AngleAxis(
                    Utility.Vector2AsAngle(dirToKiller),
                    transform.forward)
                : transform.rotation);
        if (corpse != null)
        {
            spurt.SetParent(corpse);
            spurt = null;
            corpse = null;
        }

    }

    private void OnDeath(object sender, EventArgs args)
    {
        DeathEvent e = (DeathEvent) args;
        if (shakeSettings.shake)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(
                shakeSettings.magnitude,
                shakeSettings.roughness,
                shakeSettings.fadeInTime,
                shakeSettings.fadeOutTime);
        }

        corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
        Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
        corpseBody.gravityScale = e.deadPlayer.Gravity / Physics2D.gravity.magnitude;
        corpseBody.velocity = e.corpsePushDirection * corpsePushFactor;
        if (spurt != null)
        {
            spurt.SetParent(corpse);
            spurt = null;
            corpse = null;
        }
    }
}