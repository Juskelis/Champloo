﻿using System;
using UnityEngine;

public class BlockListener : MonoBehaviour {
    
    [SerializeField]
    private PlayRandomSource blockSound;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

    [SerializeField]
    private Vector2 knockbackAmount;

    [SerializeField]
    private float stunTime;

    void Start()
    {
        EventDispatcher.Instance.AddListener<BlockEvent>(OnBlock);
    }

    void OnDestroy()
    {
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.RemoveListener<BlockEvent>(OnBlock);
        }
    }

    private void OnBlock(object sender, EventArgs args)
    {

        BlockEvent e = (BlockEvent) args;

        shakeSettings.Shake();

        blockSound.Play();

        Vector3 attackerPos = e.Attacker.CenterOfPlayer;
        Vector3 defenderPos = e.Blocker.CenterOfPlayer;

        Vector3 knockbackDir = (attackerPos - defenderPos).normalized;
        knockbackDir.x *= knockbackAmount.x;
        knockbackDir.y *= knockbackAmount.y;
        e.Attacker.GetStunned(stunTime);

        e.Attacker.ApplyForce(knockbackDir);
    }
}
