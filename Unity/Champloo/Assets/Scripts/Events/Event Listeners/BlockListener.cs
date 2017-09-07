using System;
using UnityEngine;

public class BlockListener : MonoBehaviour {

    [SerializeField]
    private float blockKnockback;

    [SerializeField]
    private PlayRandomSource blockSound;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

    [SerializeField]
    private Vector2 knockbackAmount;

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

        Vector3 attackerPos = e.Attacker.transform.position;
        Vector3 defenderPos = e.Blocker.transform.position;

        Vector3 knockbackDir = (attackerPos - defenderPos).normalized;
        knockbackDir.x *= knockbackAmount.x;
        knockbackDir.y *= knockbackAmount.y;
        e.Attacker.ApplyForce(knockbackDir);
    }
}
