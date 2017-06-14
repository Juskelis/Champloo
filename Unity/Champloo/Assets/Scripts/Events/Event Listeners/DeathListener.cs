using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathListener : MonoBehaviour {

    [SerializeField]
    private Transform spawnOnDeath;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

    [SerializeField]
    private float corpsePushFactor = 1f;

    void Start()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<DeathEvent>(OnDeath);
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

        Transform corpse = (Transform)Instantiate(spawnOnDeath, transform.position, transform.rotation);
        Rigidbody2D corpseBody = corpse.GetComponent<Rigidbody2D>();
        corpseBody.gravityScale = e.deadPlayer.Gravity / Physics2D.gravity.magnitude;
        corpseBody.velocity = e.corpsePushDirection * corpsePushFactor;
    }
}
