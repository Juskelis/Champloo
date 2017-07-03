using System;
using UnityEngine;

public class BlockListener : MonoBehaviour {

    [SerializeField]
    private float blockKnockback;

    [SerializeField]
    private CamerShakeSettings shakeSettings;

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

        if (shakeSettings.shake)
        {
            EZCameraShake.CameraShaker.Instance.ShakeOnce(
                shakeSettings.magnitude,
                shakeSettings.roughness,
                shakeSettings.fadeInTime,
                shakeSettings.fadeOutTime);
        }

        Vector3 attackerPos = e.Attacker.transform.position;
        Vector3 defenderPos = e.Blocker.transform.position;

        e.Attacker.ApplyForce((attackerPos - defenderPos).normalized * blockKnockback);
    }
}
